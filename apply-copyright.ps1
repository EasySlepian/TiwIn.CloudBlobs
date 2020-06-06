$header = "//-----------------------------------------------------------------------
// <copyright file=""{0}"" company=""{1}"">
// Copyright (c) {1}. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------`r`n"
function Write-Header ($file)
{

    $filename = Split-Path -Leaf $file
    
    $content = Get-Content $file -Raw
    if($content -cnotmatch "(?xis-m)^.*?(?=\bnamespace\s+TiwIn\b)")
    {    
        return;
    }

    $content = [System.Text.RegularExpressions.Regex]::Replace($content, "(?xis-m)^.*?(?=\bnamespace\s+TiwIn\b)", "");


    $fileheader = $header -f $filename,'TiwIn'

    Set-Content $file ($fileheader + $content.Trim());
    
    #$filename
    #$file
}


Get-ChildItem $PSScriptRoot -Recurse | ? { $_.Extension -like ".cs" } | % `
{
    Write-Header $_.FullName
}
