namespace DevDeadly
{
    public class GLSL
    {
        public static string vertexShaderSource =
            @"#version 330 core
            layout(location = 0) in vec3 aPosition;
            layout(location = 1) in vec2 aTexCoord;
            layout(location = 2) in vec3 aNormal;
            layout(location = 3) in float aTexLayer;
            layout(location = 4) in float aAO;

            out vec2 TexCoord;
            out float TexLayer;
            out vec3 Normal;
            out vec3 FragPos;
            out float vAO;
            out vec4 FragPosLightSpace;

            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;
            uniform mat4 lightSpaceMatrix;

            void main()
            {
                gl_Position = vec4(aPosition, 1.0) * model * view * projection;
                TexCoord = aTexCoord;
                TexLayer = aTexLayer;
                Normal = normalize(mat3(transpose(inverse(model))) * aNormal);
                FragPos = vec3(vec4(aPosition, 1.0) * model);
                vAO = aAO;
                FragPosLightSpace = vec4(aPosition, 1.0) * model * lightSpaceMatrix;
            }";

        public static string fragmentShaderSource =
            @"#version 330 core
            in vec2 TexCoord;
            in float TexLayer;
            in vec3 Normal;
            in vec3 FragPos;
            in float vAO;
            in vec4 FragPosLightSpace;
            out vec4 FragColor;

            uniform sampler2DArray atlasArray;
            uniform sampler2D shadowMap;
            uniform vec3 lightPos;
            uniform vec3 lightColor;
            uniform vec3 viewPos;
            uniform float fogStart;
            uniform float fogEnd;
            uniform float chunkAlpha;
            uniform vec3 fogColor;
            uniform float ambientStrength;

            float ShadowCalc(vec4 fragPosLS, vec3 norm, vec3 lightDir)
            {
                vec3 proj = fragPosLS.xyz / fragPosLS.w;
                proj = proj * 0.5 + 0.5;
                if (proj.z > 1.0) return 0.0;
                float bias = max(0.005 * (1.0 - dot(norm, lightDir)), 0.0015);
                float shadow = 0.0;
                vec2 texelSize = 1.0 / textureSize(shadowMap, 0);
                for (int x = -2; x <= 2; x++)
                {
                    for (int y = -2; y <= 2; y++)
                    {
                        float pcfDepth = texture(shadowMap, proj.xy + vec2(x, y) * texelSize).r;
                        shadow += proj.z - bias > pcfDepth ? 1.0 : 0.0;
                    }
                }
                return shadow / 25.0;
            }

            void main()
            {
                vec3 ambient = ambientStrength * lightColor;
                vec3 norm = normalize(Normal);
                vec3 lightDir = normalize(lightPos - FragPos);
                float diff = max(dot(norm, lightDir), 0.0);
                vec3 diffuse = diff * lightColor;
                float specularStrength = 0.3;
                vec3 viewDir = normalize(viewPos - FragPos);
                vec3 reflectDir = reflect(-lightDir, norm);
                float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
                vec3 specular = specularStrength * spec * lightColor;
                float shadow = ShadowCalc(FragPosLightSpace, norm, lightDir);
                vec3 lighting = (ambient + (1.0 - shadow) * (diffuse + specular)) * vAO;
                vec4 texColor = texture(atlasArray, vec3(TexCoord, TexLayer));
                vec4 baseColor = vec4(lighting, 1.0) * texColor;
                float dist = length(FragPos - viewPos);
                float fogFactor = clamp((dist - fogStart) / (fogEnd - fogStart), 0.0, 1.0);
                FragColor = mix(baseColor, vec4(fogColor, 1.0), fogFactor);
            }"; 

        public static string LampVert =

            @"# version 330 core
            layout(location = 0) in vec3 aPos;
            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;

            void main()
            {
                gl_Position = vec4(aPos, 1.0) * model * view * projection;
            }";

        public static string LampFrags =

            @"#version 330 core
            out vec4 FragColor;
            uniform vec3 lightColor;
            uniform vec3 objectColor;

            void main()
            {
                FragColor = vec4(lightColor, 1.0f);
            }";

        public static string InventoryVerts =

            @"#version 330 core
            layout (location = 0) in vec2 IUPosition;
            layout (location = 1) in vec2 IUCoord;
            out vec2 TexCoord;
            uniform mat4 projection; 

            void main()
            {
                gl_Position = vec4 (IUPosition, 0.0, 2.0);
                TexCoord = IUCoord;
            }";

        public static string InventoryFrags =

            @"#version 330 core
            in vec2 TexCoord;
            out vec4 FragColor;
            uniform sampler2D textureHUD;

            void main()
            {            
                FragColor = texture(textureHUD, TexCoord);
            }";

        public static string CreationVerts =

            @"#version 330 core
            layout (location = 0) in vec2 Crosition;
            layout (location = 1) in vec2 aCroods;
            out vec2 TexCoord;
            uniform mat4 projection; 

            void main()
            {
                gl_Position = vec4 (Crosition, 0.0, 1.0);
                TexCoord = aCroods;
            }";

        public static string CreationFrags =

            @"#version 330 core
            in vec2 TexCoord;
            out vec4 FragColor;
            uniform sampler2D TextureCreate;

            void main()
            {            
                FragColor = texture(TextureCreate, TexCoord);
            }";

        public static string TransparencyVerts =

            @"#version 330 core
            layout (location = 0) in vec2 aPos;

            void main()
            {
                gl_Position = vec4 (aPos, 0.0, 2.0);
            }";

        public static string TransparencyFrags =

            @"#version 330 core
            out vec4 FragColor;

            void main()
            {
                FragColor = vec4(0.2, 0.2, 0.2, 0.6);
            }";

        public static string ObjectVert =

            @"#version 330 core
            layout (location = 0) in vec3 aPos;
            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;

            void main()
            {
                gl_Position = vec4(aPos, 1.0) * model * view * projection;
            }";

        public static string ObjectFrag =

            @"#version 330 core
            out vec4 FragColor;

            void main()
            {
                FragColor = vec4(1.0, 0.4, 0.2, 1.0); 
            }";

        public static string SkyVert =
            @"#version 330 core
            layout(location = 0) in vec2 aPos;
            out vec2 vUV;
            void main()
            {
                vUV = aPos * 0.5 + 0.5;
                gl_Position = vec4(aPos, 1.0, 1.0);
            }";

        public static string SkyFrag =
            @"#version 330 core
            in vec2 vUV;
            out vec4 FragColor;

            uniform vec3 camFront;
            uniform vec3 camRight;
            uniform vec3 camUp;
            uniform float fovTan;
            uniform float aspectRatio;
            uniform vec3 sunDir;
            uniform float timeOfDay;

            float hash2(vec2 p)
            {
                return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
            }

            float noise2D(vec2 p)
            {
                vec2 i = floor(p);
                vec2 f = fract(p);
                vec2 u = f * f * (3.0 - 2.0 * f);
                float a = hash2(i);
                float b = hash2(i + vec2(1.0, 0.0));
                float c = hash2(i + vec2(0.0, 1.0));
                float d = hash2(i + vec2(1.0, 1.0));
                return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
            }

            float fbm(vec2 p)
            {
                return noise2D(p)               * 0.500
                     + noise2D(p * 2.1 + vec2(3.7, 1.3)) * 0.300
                     + noise2D(p * 4.3 + vec2(1.1, 8.2)) * 0.150
                     + noise2D(p * 8.7 + vec2(5.5, 2.9)) * 0.050;
            }

            void main()
            {
                vec2 uv = vUV * 2.0 - 1.0;
                vec3 rayDir = normalize(camFront + camRight * uv.x * fovTan * aspectRatio + camUp * uv.y * fovTan);
                float elevation = rayDir.y;

                vec3 nightTop     = vec3(0.005, 0.005, 0.025);
                vec3 nightHorizon = vec3(0.025, 0.025, 0.07);
                vec3 dawnTop      = vec3(0.08,  0.08,  0.40);
                vec3 dawnHorizon  = vec3(0.95,  0.45,  0.20);
                vec3 dayTop       = vec3(0.18,  0.48,  0.98);
                vec3 dayHorizon   = vec3(0.65,  0.82,  0.98);
                vec3 duskTop      = vec3(0.04,  0.04,  0.28);
                vec3 duskHorizon  = vec3(1.00,  0.32,  0.04);

                vec3 skyTop, skyHorizon;
                float t = timeOfDay;

                if (t < 0.2) {
                    skyTop = nightTop; skyHorizon = nightHorizon;
                } else if (t < 0.3) {
                    float f = (t - 0.2) * 10.0;
                    skyTop = mix(nightTop, dawnTop, f);
                    skyHorizon = mix(nightHorizon, dawnHorizon, f);
                } else if (t < 0.4) {
                    float f = (t - 0.3) * 10.0;
                    skyTop = mix(dawnTop, dayTop, f);
                    skyHorizon = mix(dawnHorizon, dayHorizon, f);
                } else if (t < 0.6) {
                    skyTop = dayTop; skyHorizon = dayHorizon;
                } else if (t < 0.7) {
                    float f = (t - 0.6) * 10.0;
                    skyTop = mix(dayTop, duskTop, f);
                    skyHorizon = mix(dayHorizon, duskHorizon, f);
                } else if (t < 0.8) {
                    float f = (t - 0.7) * 10.0;
                    skyTop = mix(duskTop, nightTop, f);
                    skyHorizon = mix(duskHorizon, nightHorizon, f);
                } else {
                    skyTop = nightTop; skyHorizon = nightHorizon;
                }

                float horizonBlend = 1.0 - clamp(elevation * 2.5 + 0.15, 0.0, 1.0);
                vec3 skyColor = mix(skyTop, skyHorizon, horizonBlend);

                float dayAmt   = smoothstep(0.20, 0.35, t) * (1.0 - smoothstep(0.65, 0.80, t));
                float nightAmt = 1.0 - dayAmt;
                vec3 sDir = normalize(sunDir);
                float sunDot  = dot(rayDir, sDir);
                float sunDisc = smoothstep(0.9994, 0.9998, sunDot);
                float sunGlow = smoothstep(0.930, 0.9994, sunDot) * 0.25;
                skyColor += vec3(1.0, 0.95, 0.80) * sunDisc * dayAmt;
                skyColor += vec3(1.0, 0.65, 0.30) * sunGlow * dayAmt;
                vec3 moonDir    = normalize(-sDir);
                float moonAngle = acos(clamp(dot(rayDir, moonDir), -1.0, 1.0));
                float moonRadius = 0.022;
                float moonDisc  = 1.0 - smoothstep(moonRadius * 0.85, moonRadius, moonAngle);
                float moonLimb  = 1.0 - smoothstep(moonRadius * 0.60, moonRadius * 0.85, moonAngle);
                float moonHalo1 = (1.0 - smoothstep(moonRadius, moonRadius * 3.5,  moonAngle)) * (1.0 - moonDisc);
                float moonHalo2 = (1.0 - smoothstep(moonRadius * 3.5, moonRadius * 12.0, moonAngle)) * (1.0 - moonDisc);
                skyColor += (vec3(1.00, 0.99, 0.94) * moonLimb             * 3.5
                           + vec3(0.88, 0.92, 1.00) * (moonDisc - moonLimb) * 2.8
                           + vec3(0.30, 0.40, 0.65) * moonHalo1             * 0.55
                           + vec3(0.30, 0.40, 0.65) * moonHalo2             * 0.18) * nightAmt;
                float auroraAmt = nightAmt * smoothstep(0.0, 0.30, nightAmt);
                auroraAmt *= smoothstep(-0.03, 0.08, elevation);  
                auroraAmt *= smoothstep(0.65,  0.40, elevation);  

                if (auroraAmt > 0.003)
                {
                    float aTime = timeOfDay * 18.0;
                    vec2 hDir = normalize(vec2(rayDir.x, rayDir.z) + vec2(0.0001));
                    float angle = aTime * 0.012;
                    float cosA  = cos(angle);
                    float sinA  = sin(angle);
                    vec2 rotDir = vec2(cosA * hDir.x - sinA * hDir.y,
                                       sinA * hDir.x + cosA * hDir.y);
                    vec2 baseUV = rotDir * 2.8;
                    float bandCenter = 0.13
                        + fbm(baseUV * 0.7 + vec2(aTime * 0.025, 0.0)) * 0.10
                        + fbm(baseUV * 1.4 + vec2(0.0, aTime * 0.018)) * 0.05;
                    float bandHalf = 0.10 + fbm(baseUV * 0.5 + vec2(aTime * 0.014, 3.0)) * 0.06;

                    float elev = clamp(elevation, 0.0, 1.0);
                    float maskBot = smoothstep(bandCenter - bandHalf * 0.5, bandCenter + bandHalf * 0.3, elev);
                    float maskTop = 1.0 - smoothstep(bandCenter + bandHalf * 0.5, bandCenter + bandHalf * 1.5, elev);
                    float bandMask = maskBot * maskTop;

                    vec2 streakUV1 = vec2(rotDir.x * 9.0 + aTime * 0.060,
                                          rotDir.y * 9.0 + aTime * 0.045);
                    vec2 streakUV2 = vec2(rotDir.x * 16.0 - aTime * 0.035,
                                          rotDir.y * 16.0 + aTime * 0.070);

                    float streak1 = noise2D(streakUV1) * 0.6 + noise2D(streakUV1 * 2.0) * 0.4;
                    float streak2 = noise2D(streakUV2) * 0.5 + noise2D(streakUV2 * 1.8) * 0.5;
                    float streaks = mix(streak1, streak2, 0.45);
                    streaks = smoothstep(0.20, 0.85, streaks);
                    float curtainFade = 1.0 - smoothstep(bandCenter, bandCenter + bandHalf, elev);
                    streaks = mix(0.4, 1.0, streaks * curtainFade);
                    vec2 shimUV = baseUV * 3.5 + vec2(aTime * 0.22, elev * 8.0 + aTime * 0.15);
                    float shimmer = noise2D(shimUV) * 0.5 + noise2D(shimUV * 2.2 + vec2(1.3, 0.7)) * 0.5;
                    shimmer = 0.72 + shimmer * 0.28;
                    float intensity = bandMask * streaks * shimmer;
                    float pulse = 0.75 + 0.25 * sin(aTime * 0.18 + 1.0)
                                       * cos(aTime * 0.11 + 0.5);
                    intensity *= pulse;
                    vec3 colGreen  = vec3(0.05, 0.95, 0.40);
                    vec3 colCyan   = vec3(0.05, 0.75, 0.90);
                    vec3 colPurple = vec3(0.50, 0.05, 0.85);
                    vec3 colPink   = vec3(0.85, 0.08, 0.55);
                    float cSeed1 = fbm(rotDir * 1.2 + vec2(aTime * 0.007,  0.0));
                    float cSeed2 = fbm(rotDir * 1.8 + vec2(0.0, aTime * 0.009 + 2.5));
                    vec3 upperColor = mix(colGreen, colCyan, smoothstep(0.2, 0.8, cSeed1));

                    float baseFactor = 1.0 - smoothstep(bandCenter - 0.02, bandCenter + 0.05, elev);
                    vec3 baseColor   = mix(colPurple, colPink, smoothstep(0.3, 0.7, cSeed2));

                    vec3 auroraColor = mix(upperColor, baseColor, baseFactor * 0.70);

                    float angle2  = aTime * 0.009 + 1.57;
                    float cosA2   = cos(angle2);
                    float sinA2   = sin(angle2);
                    vec2 rotDir2  = vec2(cosA2 * hDir.x - sinA2 * hDir.y,
                                         sinA2 * hDir.x + cosA2 * hDir.y);
                    vec2 baseUV2  = rotDir2 * 2.2;

                    float bc2  = 0.10 + fbm(baseUV2 * 0.6 + vec2(aTime * 0.019, 1.5)) * 0.08;
                    float bh2  = 0.08 + fbm(baseUV2 * 0.4 + vec2(2.0, aTime * 0.013)) * 0.05;
                    float mask2 = smoothstep(bc2 - bh2 * 0.4, bc2 + bh2 * 0.3, elev)
                                * (1.0 - smoothstep(bc2 + bh2 * 0.4, bc2 + bh2 * 1.3, elev));

                    vec2 streakUV3 = vec2(rotDir2.x * 11.0 + aTime * 0.041, rotDir2.y * 11.0);
                    float streaks2 = noise2D(streakUV3) * 0.7 + noise2D(streakUV3 * 2.3 + vec2(0.9)) * 0.3;
                    streaks2 = smoothstep(0.25, 0.90, streaks2);

                    float intensity2 = mask2 * streaks2 * shimmer * pulse * 0.65;

                    float cSeed3   = fbm(rotDir2 * 1.5 + vec2(aTime * 0.006 + 4.0, 1.0));
                    vec3 layer2col = mix(colCyan, colGreen, smoothstep(0.3, 0.7, cSeed3));

                    skyColor += auroraColor * intensity  * auroraAmt * 1.35;
                    skyColor += layer2col   * intensity2 * auroraAmt * 0.80;
                }

                if (nightAmt > 0.01 && elevation > -0.05)
                {
                    float theta    = acos(clamp(elevation, -1.0, 1.0));
                    float phi      = atan(rayDir.z, rayDir.x);
                    vec2 starUV    = vec2(phi * 55.0, theta * 110.0);
                    vec2 starCell  = floor(starUV);
                    vec2 starFract = fract(starUV) - 0.5;

                    float rng  = hash2(starCell);
                    float rng2 = hash2(starCell + vec2(13.7, 47.3));
                    float rng3 = hash2(starCell + vec2(91.1,  5.3));
                    float rng4 = hash2(starCell + vec2(33.3, 77.7));

                    if (step(0.982, rng) > 0.0)
                    {
                        float starSize = 0.024 + rng2 * 0.016;
                        float dist     = length(starFract);
                        float core     = smoothstep(starSize, starSize * 0.10, dist);
                        float halo     = exp(-dist * dist / (starSize * starSize * 7.0)) * 0.50;

                        float isBright = step(0.991, rng);
                        float spikeLen = starSize * 6.0;
                        float spikeH   = smoothstep(0.004, 0.0, abs(starFract.y)) * smoothstep(spikeLen, 0.0, abs(starFract.x));
                        float spikeV   = smoothstep(0.004, 0.0, abs(starFract.x)) * smoothstep(spikeLen, 0.0, abs(starFract.y));
                        vec2 diag      = vec2(starFract.x - starFract.y, starFract.x + starFract.y) * 0.707;
                        float spikeD1  = smoothstep(0.005, 0.0, abs(diag.y)) * smoothstep(spikeLen * 0.65, 0.0, abs(diag.x));
                        float spikeD2  = smoothstep(0.005, 0.0, abs(diag.x)) * smoothstep(spikeLen * 0.65, 0.0, abs(diag.y));
                        float sparkle     = isBright * ((spikeH + spikeV) * 2.2 + (spikeD1 + spikeD2) * 1.1);
                        float brightHalo  = isBright * exp(-dist * dist / (starSize * starSize * 22.0)) * 0.9;
                        float twinkle     = 0.65 + 0.35 * sin(timeOfDay * 550.0 + rng * 31.4) * cos(timeOfDay * 310.0 + rng4 * 17.8);
                        float twinkleSp   = 0.50 + 0.50 * sin(timeOfDay * 900.0 + rng2 * 44.0);

                        vec3 starColor;
                        if      (rng3 < 0.22) starColor = vec3(0.55, 0.80, 1.00);
                        else if (rng3 < 0.44) starColor = vec3(0.78, 0.55, 1.00);
                        else if (rng3 < 0.60) starColor = vec3(1.00, 0.93, 0.50);
                        else if (rng3 < 0.75) starColor = vec3(0.65, 0.95, 1.00);
                        else                  starColor = vec3(1.00, 1.00, 1.00);

                        float brightness = (rng - 0.982) * 75.0;
                        skyColor += starColor * (core + halo + brightHalo + sparkle * twinkleSp) * brightness * twinkle * nightAmt;
                    }
                }

                FragColor = vec4(skyColor, 1.0);
            }";

        public static string ShadowVert =
            @"#version 330 core
            layout(location = 0) in vec3 aPosition;
            uniform mat4 model;
            uniform mat4 lightSpaceMatrix;
            void main()
            {
                gl_Position = vec4(aPosition, 1.0) * model * lightSpaceMatrix;
            }";

        public static string ShadowFrag =
            @"#version 330 core
            void main() {}";

        public static string Loader_Shader =
            @"#version 330 core
            in vec3 aTexCoord;
            in vec3 aNormal;
            in vec3 aVertexArrayObject;
            in vec3 model;
            in vec3 view;
            in vec3 projection;

            layout(location = 0) vec3;
            layout(location = 1) vec2;

            float specular = aTexCoord * aNormal;
            rez_color = vec4(aVertexArrayObject * 2/ 23);
            FragColor = vec3(rez_color(model * view * projection) 1.0f);
";

    }
}