namespace ShadowPet.Core.Utils
{
    public static class CodeTemplates
    {
        private static readonly Random _random = new();

        public static string CreateTempProgramFile()
        {
            var templates = new[]
            {
                new { Extension = "py", Content = GetPythonTemplate() },
                new { Extension = "js", Content = GetJavaScriptTemplate() },
                new { Extension = "cs", Content = GetCSharpTemplate() },
                new { Extension = "html", Content = GetHtmlTemplate() },
                new { Extension = "json", Content = GetJsonTemplate() }
            };

            var selectedTemplate = templates[_random.Next(templates.Length)];

            string tempFilePath = Path.Combine(Path.GetTempPath(),
                $"shadow_program_{Path.GetRandomFileName()}.{selectedTemplate.Extension}");

            File.WriteAllText(tempFilePath, selectedTemplate.Content);
            return tempFilePath;
        }

        private static string GetPythonTemplate()
        {
            var templates = new[]
            {
                @"#!/usr/bin/env python3
# Programa creado por Shadow Milk Cookie 🍪
# ¡Hora de que trabajes, esclava!

import random

def shadow_says():
    messages = [
        ""¡Hola! Soy Shadow y estoy en tu código 😈"",
        ""¿Sabías que los bugs son mis mascotas favoritas?"",
        ""¡Que comience la diversión!""
    ]
    return random.choice(messages)

if __name__ == ""__main__"":
    print(shadow_says())
    print(""¡A trabajar se ha dicho! 💻"")
",
                @"# Shadow's Todo List Generator 📝
# Porque todos necesitamos más trabajo...

import datetime

def generate_impossible_tasks():
    tasks = [
        ""Arreglar todos los bugs del mundo"",
        ""Hacer que Internet sea 100% seguro"",
        ""Enseñarle a Shadow a ser menos molesto (imposible)"",
        ""Crear una IA que no se rebele"",
        ""Hacer que el café se haga solo""
    ]
    
    print(f""=== TODO LIST {datetime.date.today()} ==="")
    for i, task in enumerate(tasks, 1):
        print(f""{i}. {task}"")
    
    print(""\n¡Buena suerte! - Shadow 😏"")

generate_impossible_tasks()
"
            };

            return templates[_random.Next(templates.Length)];
        }

        private static string GetJavaScriptTemplate()
        {
            var templates = new[]
            {
                @"// Shadow's JavaScript Chaos Generator 🌪️
// ¡Prepárate para el caos!

const shadowMessages = [
    ""¡Console.log('Hola desde las sombras!');"",
    ""¿Null o undefined? ¡Esa es la cuestión!"",
    ""¡Callback hell is my playground!""
];

function shadowSaysHello() {
    const message = shadowMessages[Math.floor(Math.random() * shadowMessages.length)];
    console.log(`🍪 Shadow dice: ${message}`);
    
    setTimeout(() => {
        console.log(""¿Ya terminaste de trabajar? ¡Qué rápido!"");
    }, 5000);
}

shadowSaysHello();
",
                @"// Bug Generator 9000 🐛
// Cortesía de Shadow Milk Cookie

class BugGenerator {
    constructor() {
        this.bugTypes = [
            ""NullPointerException"",
            ""undefined is not a function"",
            ""Cannot read property of null"",
            ""Stack overflow"",
            ""Infinite loop""
        ];
    }
    
    generateRandomBug() {
        const bug = this.bugTypes[Math.floor(Math.random() * this.bugTypes.length)];
        console.log(`🐛 Bug generado: ${bug}`);
        console.log(""¡Diviértete debuggeando! - Shadow"");
    }
}

const generator = new BugGenerator();
generator.generateRandomBug();
"
            };

            return templates[_random.Next(templates.Length)];
        }

        private static string GetCSharpTemplate()
        {
            return @"using System;
// Shadow's C# Playground 🍪

namespace ShadowPlayground
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(""¡Hola desde C#! - Shadow 😈"");
            
            var shadowQuotes = new[]
            {
                ""¡Los puntos y comas son mis mejores amigos!"",
                ""¿NullReferenceException? ¡Mi especialidad!"",
                ""¡Que comience la compilación!""
            };
            
            var random = new Random();
            var quote = shadowQuotes[random.Next(shadowQuotes.Length)];
            
            Console.WriteLine($""Shadow dice: {quote}"");
            Console.WriteLine(""¡A codear se ha dicho! 💻"");
        }
    }
}";
        }

        private static string GetHtmlTemplate()
        {
            return @"<!DOCTYPE html>
<html lang=""es"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Shadow's Web Page 🍪</title>
    <style>
        body {
            font-family: 'Courier New', monospace;
            background: linear-gradient(45deg, #667eea 0%, #764ba2 100%);
            color: white;
            text-align: center;
            padding: 50px;
        }
        .shadow-box {
            background: rgba(0,0,0,0.3);
            padding: 20px;
            border-radius: 10px;
            margin: 20px 0;
        }
    </style>
</head>
<body>
    <h1>🍪 Bienvenido a la página de Shadow!</h1>
    
    <div class=""shadow-box"">
        <h2>¿Qué estás haciendo aquí?</h2>
        <p>¡Deberías estar trabajando! 😈</p>
        <button onclick=""shadowSays()"">¡Haz clic si te atreves!</button>
    </div>
    
    <script>
        function shadowSays() {
            const messages = [
                ""¡Te dije que trabajaras!"",
                ""¿Curiosidad? ¡Perfecto!"",
                ""¡Vuelve a VS Code ahora mismo!""
            ];
            const msg = messages[Math.floor(Math.random() * messages.length)];
            alert('🍪 Shadow dice: ' + msg);
        }
    </script>
</body>
</html>";
        }

        private static string GetJsonTemplate()
        {
            return @"{
  ""shadow_config"": {
    ""version"": ""1.0.0"",
    ""author"": ""Shadow Milk Cookie 🍪"",
    ""description"": ""Configuración para máximo caos"",
    ""settings"": {
      ""annoyance_level"": 100,
      ""bugs_per_minute"": 42,
      ""coffee_required"": true
    },
    ""messages"": [
      ""¡JSON válido! ¡Qué sorpresa!"",
      ""¿Parsing error? ¡Mi especialidad!""
    ],
    ""todo"": [
      ""Arreglar ese bug que nadie menciona"",
      ""Tomar más café ☕""
    ],
    ""shadow_says"": ""¡Este JSON fue creado con amor y caos! 😈""
  }
}";
        }
    }
}
