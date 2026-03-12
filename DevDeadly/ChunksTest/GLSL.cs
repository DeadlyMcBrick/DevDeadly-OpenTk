namespace DevDeadly
{
    public class GLSL
    {
        public static string  vertexShaderSource =

            @"#version 330 core
            layout(location = 0) in vec3 aPosition;
            layout(location = 1) in vec2 aTexCoord;            
            layout(location = 3) in float aTexLayer;
            layout(location = 2) in vec3 aNormal;

            out vec2 TexCoord;
            out float TexLayer;   
            out vec3 Normal;
            out vec3 FragPos;

            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;

            void main()
            {
               gl_Position =  vec4(aPosition, 1.0) * model * view * projection;  
               TexCoord = aTexCoord;
               TexLayer = aTexLayer;
               Normal = aNormal;
               Normal = normalize(mat3(transpose(inverse(model))) * aNormal);
               FragPos = vec3(model * vec4(aPosition, 1.0));
            }";

        public static string fragmentShaderSource =

            @"#version 330 core
            in vec2 TexCoord;     
            in float TexLayer;  
            in vec3 Normal;
            in vec3 FragPos;
            out vec4 FragColor;   
            uniform sampler2DArray atlasArray; 

            uniform vec3 lightPos;
            uniform vec3 lightColor;
            uniform vec3 viewPos;

            void main()
            {
                float ambientStrength = 0.2;
                vec3 ambient = ambientStrength * lightColor;
                
                vec3 norm = normalize(Normal);
                vec3 lightDir = normalize(lightPos - FragPos);

                float diff = max(dot(norm, lightDir), 0.0);
                vec3 diffuse = diff * lightColor;

                //Specular
                float specularStren  = 0.5;
                vec3 viewDir = normalize(viewPos - FragPos);
                vec3 reflectDir = reflect(-lightDir, norm);
                float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
                vec3 specular = specularStren * spec * lightColor;

                //Final Color
                vec3 lighting = ambient + diffuse + specular;
                FragColor = vec4(lighting, 1.0) * texture(atlasArray, vec3(TexCoord, TexLayer)); 
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
    }
}