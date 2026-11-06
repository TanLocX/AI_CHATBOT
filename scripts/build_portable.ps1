# ==============================================================================
# Script tu dong hoa dong goi ban "Tai ve la dung ngay" (Portable Release)
# Du an: FluentChat AI Studio (AI_Chatbot)
# ==============================================================================

$ErrorActionPreference = "Stop"

$RootDir = Resolve-Path (Join-Path $PSScriptRoot "..")
Set-Location $RootDir

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host " BAT DAU DONG GOI BAN PORTABLE RELEASE (C# WINFORMS) " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

# 1. Kiem tra nuget.exe
$NugetExe = Join-Path $RootDir "nuget.exe"
if (!(Test-Path $NugetExe)) {
    Write-Host "[-] Dang tai nuget.exe..." -ForegroundColor Yellow
    [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12
    Invoke-WebRequest -Uri "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile $NugetExe
}

# 2. Khoi phuc NuGet Packages
Write-Host "[1/5] Dang khoi phuc cac thu vien NuGet qua nuget.exe..." -ForegroundColor Green
& $NugetExe restore AI_Chatbot.sln

# 3. Tim duong dan MSBuild
Write-Host "[2/5] Dang tim kiem MSBuild..." -ForegroundColor Green
$VsWhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
$MsBuildExe = $null

if (Test-Path $VsWhere) {
    $vsPath = & $VsWhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
    if ($vsPath) {
        $candidate = Join-Path $vsPath "MSBuild\Current\Bin\MSBuild.exe"
        if (Test-Path $candidate) { $MsBuildExe = $candidate }
    }
}

if (!$MsBuildExe) {
    $cmd = Get-Command msbuild.exe -ErrorAction SilentlyContinue
    if ($cmd) { $MsBuildExe = $cmd.Source }
}

if (!$MsBuildExe) {
    throw "Khong tim thay MSBuild.exe tren he thong! Vui long cai dat Visual Studio 2019/2022."
}
Write-Host "       Tim thay MSBuild tai: $MsBuildExe" -ForegroundColor Gray

# 4. Bien dich project o che do Release
Write-Host "[3/5] Dang bien dich du an o che do Release..." -ForegroundColor Green
& $MsBuildExe AI_Chatbot.sln /p:Configuration=Release /p:Platform="Any CPU" /verbosity:minimal /nologo
if ($LASTEXITCODE -ne 0) {
    throw "Bien dich that bai voi ma loi $LASTEXITCODE!"
}

# 5. Kiem tra va tai du lieu OCR (tessdata) neu chua co
$ReleaseDir = Join-Path $RootDir "AI_Chatbot\bin\Release"
$TessDataDir = Join-Path $ReleaseDir "tessdata"
if (!(Test-Path $TessDataDir)) {
    New-Item -ItemType Directory -Path $TessDataDir -Force | Out-Null
}

$vieFile = Join-Path $TessDataDir "vie.traineddata"
$engFile = Join-Path $TessDataDir "eng.traineddata"
$webClient = New-Object System.Net.WebClient

if (!(Test-Path $vieFile)) {
    Write-Host "       Dang tai vie.traineddata (OCR Tieng Viet)..." -ForegroundColor Yellow
    $webClient.DownloadFile("https://github.com/tesseract-ocr/tessdata_fast/raw/main/vie.traineddata", $vieFile)
}
if (!(Test-Path $engFile)) {
    Write-Host "       Dang tai eng.traineddata (OCR Tieng Anh)..." -ForegroundColor Yellow
    $webClient.DownloadFile("https://github.com/tesseract-ocr/tessdata_fast/raw/main/eng.traineddata", $engFile)
}

# 6. Chuan bi thu muc staging dist/AI_Chatbot_Portable
Write-Host "[4/5] Dang gom cac file doc lap vao thu muc phan phoi..." -ForegroundColor Green
$DistDir = Join-Path $RootDir "dist\AI_Chatbot_Portable"
if (Test-Path $DistDir) {
    Remove-Item -Path $DistDir -Recurse -Force
}
New-Item -ItemType Directory -Path $DistDir -Force | Out-Null

# Copy file exe, config
Copy-Item (Join-Path $ReleaseDir "AI_Chatbot.exe") -Destination $DistDir -Force
Copy-Item (Join-Path $ReleaseDir "AI_Chatbot.exe.config") -Destination $DistDir -Force

# Copy toan bo DLL
Get-ChildItem -Path $ReleaseDir -Filter "*.dll" | ForEach-Object {
    Copy-Item $_.FullName -Destination $DistDir -Force
}

# Copy cac thu muc con
if (Test-Path (Join-Path $ReleaseDir "x86")) {
    Copy-Item (Join-Path $ReleaseDir "x86") -Destination $DistDir -Recurse -Force
}
if (Test-Path (Join-Path $ReleaseDir "x64")) {
    Copy-Item (Join-Path $ReleaseDir "x64") -Destination $DistDir -Recurse -Force
}
if (Test-Path (Join-Path $ReleaseDir "Resources")) {
    Copy-Item (Join-Path $ReleaseDir "Resources") -Destination $DistDir -Recurse -Force
}
if (Test-Path (Join-Path $ReleaseDir "tessdata")) {
    Copy-Item (Join-Path $ReleaseDir "tessdata") -Destination $DistDir -Recurse -Force
}

# Tao file Huong dan su dung nhanh di kem
$HdPath = Join-Path $DistDir "HUONG_DAN_SU_DUNG.txt"
$lines = @(
    "==============================================================================",
    " FLUENTCHAT AI STUDIO - DESKTOP ASSISTANT (BAN PORTABLE CHAY NGAY)",
    "==============================================================================",
    "",
    "Chao mung ban su dung phan mem FluentChat AI Studio!",
    "Ban dong goi nay da tich hop day du moi thu vien va du lieu can thiet.",
    "",
    "1. CACH KHOI CHAY:",
    "   - Nhap dup chuot vao file: AI_Chatbot.exe de mo ung dung ngay lap tuc.",
    "   - Khong can cai dat Visual Studio hay bat ky cong cu lap trinh nao.",
    "",
    "2. CAC TINH NANG CHINH:",
    "   - Chat AI Local (Ollama): Chay offline hoan toan tren may tinh cua ban.",
    "   - Chat AI Cloud (Google Gemini): Can bam nut [Cau hinh API Key] de nhap key mien phi.",
    "   - Doc tai lieu: Ho tro Word (.docx), PDF (.pdf), Excel (.xlsx), Text, CSV, Markdown.",
    "   - Nhan dien chu OCR: Quet chu Tieng Viet va Tieng Anh tu anh offline.",
    "   - Xuat du lieu bang ra Excel: Phan tich anh bang bieu tu dong xuat file Excel.",
    "",
    "Chuc ban co trai nghiem tuyet voi!",
    "=============================================================================="
)
[System.IO.File]::WriteAllLines($HdPath, $lines)

# 7. Nen thanh file ZIP
Write-Host "[5/5] Dang nen thanh file ZIP Portable..." -ForegroundColor Green
$ZipOutput = Join-Path $RootDir "dist\AI_Chatbot_Portable_v1.0.zip"
if (Test-Path $ZipOutput) {
    Remove-Item -Path $ZipOutput -Force
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory($DistDir, $ZipOutput)

$zipItem = Get-Item $ZipOutput
$zipSizeMB = [math]::Round($zipItem.Length / 1MB, 2)

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host " DONG GOI THANH CONG!" -ForegroundColor Green
Write-Host " File ZIP Portable: $ZipOutput" -ForegroundColor White
Write-Host " Kich thuoc goi: $zipSizeMB MB" -ForegroundColor White
Write-Host " Nguoi dung chi can tai file zip nay ve, giai nen va nhap dup AI_Chatbot.exe la chay duoc ngay!" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan
