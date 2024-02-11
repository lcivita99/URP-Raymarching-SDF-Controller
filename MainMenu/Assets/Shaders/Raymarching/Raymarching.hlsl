// MIT License

// Copyright (c) 2021 NedMakesGames

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#ifndef CUSTOM_LIGHTING_INCLUDED 
#define CUSTOM_LIGHTING_INCLUDED

#define MAX_STEPS 100
#define MAX_DIST 100
#define SURF_DIST 0.001
#define BLENDINESS 0.2
#define SHAPE_COUNT 10

#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
#pragma multi_compile _ _SHADOWS_SOFT
#pragma multi_compile _ _ADDITIONAL_LIGHTS
#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS

// This is a neat trick to work around a bug in the shader graph when
// enabling shadow keywords. Created by @cyanilux
// https://github.com/Cyanilux/URP_ShaderGraphCustomLighting

#ifndef SHADERGRAPH_PREVIEW
    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
    #if (SHADERPASS != SHADERPASS_FORWARD)
        #undef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
    #endif
#endif

struct CustomLightingData {
    // Position and orientation
    float3 positionWS;
    float3 normalWS;
    float3 viewDirectionWS;
    float4 shadowCoord;
    float2 uv;
    float3 camPos;

    // Surface
    float3 albedo;
    float smoothness;
    float ambientOcclusion;
};

// Translate a [0, 1] smoothness value to an exponent 
float GetSmoothnessPower(float rawSmoothness) {
    return exp2(10 * rawSmoothness + 1);
}

#ifndef SHADERGRAPH_PREVIEW

float3 CustomLightHandling(CustomLightingData d, Light light) {

    float3 radiance = light.color * (light.distanceAttenuation * light.shadowAttenuation);
    
    float diffuse = saturate(dot(d.normalWS, light.direction));
    float specularDot = saturate(dot(d.normalWS, normalize(light.direction + d.viewDirectionWS)));
    float specular = pow(specularDot, GetSmoothnessPower(d.smoothness)) * diffuse;

    float3 color = d.albedo * radiance * (diffuse + specular);
    color = float3 (1., 1., 1.);
    
    return color;
}
#endif

// smooth minimum function for smoothly blending mins (& making metashape)
float smin(float a, float b, float k) {
    float h = max(k - abs(a - b), 0.0) / k;
    return min(a, b) - h * h * h * k * (1.0 / 6.0);
}

float sdBoxFrame( float3 p, float3 b, float e )
{
    p = abs(p  )-b;
    float3 q = abs(p+e)-e;
    return min(min(
        length(max(float3(p.x,q.y,q.z),0.0))+min(max(p.x,max(q.y,q.z)),0.0),
        length(max(float3(q.x,p.y,q.z),0.0))+min(max(q.x,max(p.y,q.z)),0.0)),
        length(max(float3(q.x,q.y,p.z),0.0))+min(max(q.x,max(q.y,p.z)),0.0));
}

// TotalSDF
float TotalSDF(float3 pos)
{
    // float3 shapes[];
    
    for (int i=0; i < SHAPE_COUNT; i++)
    {
        
    }

    float3 shapePos1 = float3(0, 0, 0);
    float3 shapePos2 = float3(0.1, 0.23, 0.23);
    
    float dist1 = length(pos - shapePos1) - 0.2;
    // float dist2 = length(pos - shapePos2) - 0.2;
    float dist2 = sdBoxFrame(pos - shapePos2, float3(0.2,0.3,0.1), 0.02);

    return smin(dist1, dist2, BLENDINESS);
}

float Raymarch(float3 rayOrigin, float3 rayDirection)
{
    float distanceFromOrigin = 0;
    float distanceFromScene;
    for (int i=0; i < MAX_STEPS; i++)
    {
        float3 pos = rayOrigin + distanceFromOrigin * rayDirection;
        distanceFromScene = TotalSDF(pos);
        distanceFromOrigin += distanceFromScene;
        if (distanceFromScene < SURF_DIST || distanceFromOrigin > MAX_DIST) break;
    }

    return distanceFromOrigin;
}

float3 GetNormal(float3 p)
{
    float2 e = float2(1e-2, 0);
    float3 n = TotalSDF(p) - float3 (
        TotalSDF(p-e.xyy),
        TotalSDF(p-e.yxy),
        TotalSDF(p-e.yyx)
        );
    return normalize(n);
}

float3 CalculateCustomLighting(CustomLightingData d) {
    float3 rayOrigin = d.camPos;
    float3 rayDir = normalize(d.positionWS - rayOrigin);

    float dist = Raymarch(rayOrigin, rayDir);

    float3 col;

    if (dist >= MAX_DIST)
    {
        discard;
    } else
    {
        float3 p = rayOrigin + rayDir * dist;
        float3 n = GetNormal(p);
        col.rgb = n;
    }
    
    return col;
}

void CalculateCustomLighting_float(float3 Position, float3 Normal, float3 ViewDirection,
    float3 Albedo, float Smoothness, float AmbientOcclusion,
    float2 LightmapUV, float3 CamPos,
    out float3 Color) {

    CustomLightingData d;
    d.positionWS = Position;
    d.normalWS = Normal;
    d.viewDirectionWS = ViewDirection;
    d.albedo = Albedo;
    d.smoothness = Smoothness;
    d.ambientOcclusion = AmbientOcclusion;
    d.uv = LightmapUV;
    d.camPos = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.));

    Color = CalculateCustomLighting(d);
}

#endif