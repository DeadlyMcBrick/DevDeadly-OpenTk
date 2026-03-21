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

        public static string CloudVerts =

            @"#version 330 core
            layout(location = 0) in vec3 Cloud;
            layout(location = 1) in vec2 aTexCoordCloud;
            uniform mat4 modelcloud;
            uniform mat4 viewcloud;
            uniform mat4 projectioncloud;
            out vec2 TexCoord;

            void main()
            {
                gl_Position = vec4(Cloud, 1.0) * modelcloud * viewcloud * projectioncloud;
                TexCoord = aTexCoordCloud;
            }";

        public static string CloudFrags =

            @"#version 330 core
            in vec2 TexCoord;
            out vec4 FragColor;

            void main()
            {
                FragColor = vec4(0.95, 0.95, 1.0, 0.4);       
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

                float dayAmt = smoothstep(0.20, 0.35, t) * (1.0 - smoothstep(0.65, 0.80, t));

                vec3 sDir = normalize(sunDir);
                float sunDot  = dot(rayDir, sDir);
                float sunDisc = smoothstep(0.9994, 0.9998, sunDot);
                float sunGlow = smoothstep(0.930, 0.9994, sunDot) * 0.25;
                skyColor += vec3(1.0, 0.95, 0.80) * sunDisc * dayAmt;
                skyColor += vec3(1.0, 0.65, 0.30) * sunGlow * dayAmt;

                float moonDot  = dot(rayDir, -sDir);
                float moonDisc = smoothstep(0.9988, 0.9994, moonDot);
                skyColor += vec3(0.88, 0.92, 1.0) * moonDisc * (1.0 - dayAmt);

                float nightAmt = 1.0 - dayAmt;
                if (nightAmt > 0.01 && elevation > -0.05)
                {
                    float theta  = acos(clamp(elevation, -1.0, 1.0));
                    float phi    = atan(rayDir.z, rayDir.x);
                    vec2 starUV   = vec2(phi * 47.747, theta * 95.493);
                    vec2 starCell = floor(starUV);
                    vec2 starFract = fract(starUV) - 0.5;

                    float rng  = hash2(starCell);
                    float rng2 = hash2(starCell + vec2(13.7, 47.3));

                    float starSize  = 0.06 + rng2 * 0.08;
                    float starShape = smoothstep(starSize, starSize * 0.3, length(starFract));
                    float starBright = step(0.982, rng) * (rng - 0.982) * 55.0;
                    float twinkle   = 0.75 + 0.25 * sin(timeOfDay * 800.0 + rng * 25.13);

                    vec3 starColor = vec3(
                        0.82 + rng2 * 0.18,
                        0.87 + rng2 * 0.10,
                        0.92 + rng2 * 0.08
                    );

                    skyColor += starColor * starBright * starShape * twinkle * nightAmt;

                    float brightBright = step(0.997, rng) * (rng - 0.997) * 200.0;
                    skyColor += vec3(1.0, 1.0, 0.9) * brightBright * nightAmt * 2.0;
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
    }
}