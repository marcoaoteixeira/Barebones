#Requires -Version 2.0
<#
    .SYNOPSIS
    Creates a EF core migration

    .DESCRIPTION
    Creates a new migration for the application.

    .LINK
	Project home: https://github.com/marcoaoteixeira/nameless

	.NOTES
	Author:
	Version: 1.0.0
	
	This script is designed to be called from PowerShell.
#>
[CmdletBinding()]
Param (
	[Parameter(Mandatory = $true, HelpMessage = "The name of the migration.")]
	[Alias("n")]
    [String]$MigrationName = $null,

    [Parameter(Mandatory = $false, HelpMessage = "Whether should show prompt for errors")]
    [Switch]$PromptOnError = $false
)

# Turn on Strict Mode to help catch syntax-related errors.
#   This must come after a script's/function's param section.
#   Forces a Function to be the first non-comment code to appear in a PowerShell Module.
Set-StrictMode -Version Latest

#==========================================================
# Define any necessary global variables, such as file paths.
#==========================================================

# Gets the script file name, without extension.
$THIS_SCRIPT_NAME = [System.IO.Path]::GetFileNameWithoutExtension($MyInvocation.MyCommand.Definition)

# Get the directory that this script is in.
$THIS_SCRIPT_DIRECTORY_PATH = Split-Path $script:MyInvocation.MyCommand.Path

#==========================================================
# Define functions used by the script.
#==========================================================

# Catch any exceptions Thrown, display the error message, wait for input if appropriate, and then stop the script.
Trap [Exception] {
    $errorMessage = $_
    Write-Host "An error occurred while executing the script:`n$errorMessage`n" -Foreground Red
    
    If ($PromptOnError) {
        Write-Host "Press any key to continue ..."
        $userInput = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyUp")
    }

    Break;
}

# PowerShell v2.0 compatible version of [String]::IsNullOrWhitespace.
Function Test-StringIsNullOrWhitespace([String]$string) {
    Return [String]::IsNullOrWhiteSpace($string)
}

Function Load-EnvFile() {
    # Load the .env file if it exists
    $envFilePath = Join-Path $THIS_SCRIPT_DIRECTORY_PATH ".env"
    If (Test-Path $envFilePath) {
        Write-Verbose "Loading environment variables from $envFilePath"
        Get-Content $envFilePath | ForEach-Object {
            If ($_ -match '^\s*([^#][^=]*)=(.*)$') {
                [System.Environment]::SetEnvironmentVariable($matches[1], $matches[2])
            }
        }
    } Else {
        Write-Verbose "No .env file found at $envFilePath"
    }
}

#==========================================================
# Perform the script tasks.
#==========================================================

# Display the time that this script started running.
$scriptStartTime = Get-Date
Write-Verbose "[$($THIS_SCRIPT_NAME)] Starting at $($scriptStartTime.TimeOfDay.ToString())."

# Display the version of PowerShell being used to run the script, as this can help solve some problems that are hard to reproduce on other machines.
Write-Verbose "Using PowerShell Version: $($PSVersionTable.PSVersion.ToString())."

Try {
    Load-EnvFile
    	
	dotnet ef migrations add $MigrationName -s $env:STARTUP_PROJECT_PATH -p $env:DB_CONTEXT_PROJECT_PATH -o $env:MIGRATION_OUTPUT_PATH -v
} Finally {
    Write-Verbose "[$($THIS_SCRIPT_NAME)] Performing cleanup..."
}

# Display the time that this script finished running, and how long it took to run.
$scriptFinishTime = Get-Date
$scriptElapsedTimeInSeconds = ($scriptFinishTime - $scriptStartTime).TotalSeconds.ToString()
Write-Verbose "[$($THIS_SCRIPT_NAME)] Finished at $($scriptFinishTime.TimeOfDay.ToString())."
Write-Verbose "Completed in $scriptElapsedTimeInSeconds seconds."