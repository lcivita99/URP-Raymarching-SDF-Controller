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
#define BLENDINESS 0.3
// #define SHAPE_COUNT 10

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

    float3 pos1;
    float3 pos2;
    float3 pos3;
    float3 pos4;
    float3 pos5;
    float3 pos6;
    float3 pos7;
    float3 pos8;

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
float smin8(float distances[8], float k) {
    float smoothMin = distances[0]; // Initialize with the first distance
    for (int i = 1; i < 8; i++) {
        smoothMin = smin(smoothMin, distances[i], k);
    }
    return smoothMin;
}

// SDFs from Inigo Quilez https://iquilezles.org/articles/distfunctions/

float sdSphere( float3 p, float s )
{
    return length(p)-s;
}

float sdRoundBox( float3 p, float3 b, float r )
{
    float3 q = abs(p) - b;
    return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0) - r;
}

float sdCapsule( float3 p, float3 a, float3 b, float r )
{
    float3 pa = p - a, ba = b - a;
    float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
    return length( pa - ba*h ) - r;
}

float sdRoundCone( float3 p, float r1, float r2, float h )
{
    // sampling independent computations (only depend on shape)
    float b = (r1-r2)/h;
    float a = sqrt(1.0-b*b);

    // sampling dependant computations
    float2 q = float2( length(p.xz), p.y );
    float k = dot(q,float2(-b,a));
    if( k<0.0 ) return length(q) - r1;
    if( k>a*h ) return length(q-float2(0.0,h)) - r2;
    return dot(q, float2(a,b) ) - r1;
}

// TotalSDF
float TotalSDF(float3 main_pos, float3 pos[8])
{
    float shapeDists[8] = {
        sdSphere(main_pos - pos[0], 0.05),
        sdSphere(main_pos - pos[1], 0.05),
        sdSphere(main_pos - pos[2], 0.05),
        sdSphere(main_pos - pos[3], 0.05),
        sdCapsule(main_pos - pos[4], float3(0, -0.1, 0), float3(0, 0.1, 0), 0.1 ),
        sdCapsule(main_pos - pos[5], float3(0, 0, -0.15), float3(0, 0, 0.15), 0.1 ),
        sdCapsule(main_pos - pos[6], float3(-0.15, 0, 0), float3(0.15, 0, 0), 0.1 ),
        sdSphere(main_pos - pos[7], 0.05)
        // sdSphere(main_pos - pos[7], 0.05)
        // sdSphere(pos[1], 0.5),
        // sdSphere(pos[2], 0.5),
        // sdSphere(pos[3], 0.5),
        // sdSphere(pos[4], 0.5),
        // sdSphere(pos[5], 0.5),
        // sdSphere(pos[6], 0.5),
        // sdSphere(pos[7], 0.5)
    };
    
    // float dist1 = length(main_pos - shapePos1) - 0.2;
    // float dist2 = length(main_pos - shapePos2) - 0.2;
    // float dist2 = sdBoxFrame(pos - shapePos2, float3(0.2,0.3,0.1), 0.02);

    return smin8(shapeDists, BLENDINESS);
}

float Raymarch(float3 rayOrigin, float3 rayDirection, float3 pos8[8])
{
    float distanceFromOrigin = 0;
    float distanceFromScene;
    for (int i=0; i < MAX_STEPS; i++)
    {
        float3 pos = rayOrigin + distanceFromOrigin * rayDirection;
        distanceFromScene = TotalSDF(pos, pos8);
        distanceFromOrigin += distanceFromScene;
        if (distanceFromScene < SURF_DIST || distanceFromOrigin > MAX_DIST) break;
    }

    return distanceFromOrigin;
}

float3 GetNormal(float3 p, float3 pos8[8])
{
    float2 e = float2(1e-2, 0);
    float3 n = TotalSDF(p, pos8) - float3 (
        TotalSDF(p-e.xyy, pos8),
        TotalSDF(p-e.yxy, pos8),
        TotalSDF(p-e.yyx, pos8)
        );
    return normalize(n);
}

float3 CalculateCustomLighting(CustomLightingData d) {
    float3 rayOrigin = d.camPos;
    float3 rayDir = normalize(d.positionWS - rayOrigin);

    float3 pos8[8] = {d.pos1, d.pos2, d.pos3, d.pos4, d.pos5, d.pos6, d.pos7, d.pos8};

    float dist = Raymarch(rayOrigin, rayDir, pos8);

    float3 col;

    if (dist >= MAX_DIST)
    {
        discard;
    } else
    {
        float3 p = rayOrigin + rayDir * dist;
        float3 n = GetNormal(p, pos8);
        col.rgb = n;
    }
    
    return col;
}

void CalculateCustomLighting_float(float3 Position, float3 Normal, float3 ViewDirection,
    float3 Albedo, float Smoothness, float AmbientOcclusion,
    float2 LightmapUV, float3 CamPos,
    float3 Pos1,
    float3 Pos2,
    float3 Pos3,
    float3 Pos4,
    float3 Pos5,
    float3 Pos6,
    float3 Pos7,
    float3 Pos8,
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
    d.pos1 = Pos1;
    d.pos2 = Pos2;
    d.pos3 = Pos3;
    d.pos4 = Pos4;
    d.pos5 = Pos5;
    d.pos6 = Pos6;
    d.pos7 = Pos7;
    d.pos8 = Pos8;

    Color = CalculateCustomLighting(d);
}

#endif