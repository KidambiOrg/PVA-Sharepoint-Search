# connect to SharePoint Site
$conn = Connect-PnPOnline -url https://{TENANT NAME}.sharepoint.com/sites/{SITE NAME}/ -Interactive -ReturnConnection

# Get the current properties
Get-PnPApplicationCustomizer -Connection $conn | select *

# UpDate the properties - If not done via code
$props = "{`"BotName`":`"{DEMO BOT}`",`"DirectLineSecret`":`"DIRECT LINE TOKEN OF `"}"
Set-PnPApplicationCustomizer -Connection $conn -ClientSideComponentId [ID GOT FROM LINE 7] -Scope web -ClientSideComponentProperties  $props