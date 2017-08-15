param([string]$config)

$MAJOR_VERSION = 2;
$MINOR_VERSION = 0;
$COMMITS_COUNT = git rev-list HEAD --count;
$BUILDS_COUNT = 0;
$EXTRA_INFO = "PRE-RELEASE";
$YEAR = [System.DateTime]::UtcNow.Year;

if($config.ToLower() -eq "release")
{
    $EXTRA_INFO = "RELEASE";
}

$lastBuildSaveFile = (Split-Path -Parent $MyInvocation.MyCommand.Definition) + "\last_build.txt";
if(Test-Path $lastBuildSaveFile)
{
    $fileContent = Get-Content $lastBuildSaveFile;
    $BUILDS_COUNT = [System.UInt32]::Parse($fileContent);
    $BUILDS_COUNT++;
}

Set-Content $lastBuildSaveFile $BUILDS_COUNT;

$destinationFile = "..\Source\Properties\AssemblyInfo.cs";
$templateFile = "..\Source\Properties\AssemblyInfo_template.cs";

$newAssemblyText = Get-Content $templateFile |
    %{$_ -replace '\$MAJOR\$', ($MAJOR_VERSION) } |
    %{$_ -replace '\$MINOR\$', ($MINOR_VERSION) } |
    %{$_ -replace '\$COMMITS_COUNT\$', ($COMMITS_COUNT) } |
    %{$_ -replace '\$BUILDS_COUNT\$', ($BUILDS_COUNT) } |
    %{$_ -replace '\$EXTRA_INFO\$', ($EXTRA_INFO) } |
    %{$_ -replace '\$YEAR\$', ($YEAR) };

$newAssemblyText > $destinationFile;

exit 0;