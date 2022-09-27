# connect to SharePoint Site
$conn = Connect-PnPOnline -url https://{TENANT NAME}.sharepoint.com/sites/{SITE NAME}/ -Interactive -ReturnConnection

# Add site catalog
Add-PnPSiteCollectionAppCatalog -Connection $conn -Site "https://{TENANT NAME}.sharepoint.com/sites/{SITE NAME}"