Here are the step-by-step instructions for setting up the necessary credentials in the Google Cloud Console. This process will generate the `credentials.json` file that the application needs to authenticate with your Google account.

---
### **Step 1: Select or Create a Google Cloud Project**

First, you need a project to associate with your application.

1.  Go to the [Google Cloud Console](https://console.cloud.google.com/).
2.  If you have an existing project you'd like to use, select it from the project dropdown at the top of the page.
3.  If you need a new project, click the project dropdown, then click **New Project**. Give it a name (e.g., "Drive CLI Tool") and click **Create**.

---
### **Step 2: Enable the Google Drive API**

You must enable the Drive API for your project so the application can communicate with it.

1.  In the Google Cloud Console, use the search bar at the top to search for "**Google Drive API**" and select it from the results.
2.  On the Google Drive API page, click the **Enable** button. If it's already enabled, you can move to the next step.

---
### **Step 3: Configure the OAuth Consent Screen**

The consent screen is what you will see in your browser the first time you run the app, asking for permission to access your Drive.

1.  In the search bar, search for "**OAuth consent screen**" and select it.
2.  You'll be asked for a **User Type**. Choose **External** and click **Create**.
3.  On the next page, fill in the required app information:
    * **App name**: Something descriptive, like "My Drive CLI Downloader".
    * **User support email**: Select your email address.
    * **Developer contact information**: Enter your email address again.
4.  Click **Save and Continue**.
5.  On the **Scopes** page, click **Add or Remove Scopes**.
    * In the filter box, type `drive.readonly`.
    * Find and check the box for `.../auth/drive.readonly`.
    * Click **Update**, then click **Save and Continue**.
6.  On the **Test users** page, click **Add Users**.
    * Enter the email address of the Google account you'll be using to access the Drive. This is a security measure for apps that aren't published.
    * Click **Add**, then click **Save and Continue**.
7.  Review the summary and click **Back to Dashboard**.

---
### **Step 4: Create the OAuth 2.0 Credentials**

This is the final step where you generate the actual credentials file.

1.  Navigate to the "**Credentials**" page (you can search for it or find it in the "APIs & Services" menu).
2.  At the top of the page, click **+ Create Credentials** and select **OAuth client ID**.
3.  For the **Application type**, select **Desktop app** from the dropdown menu.
4.  Give the client ID a name (e.g., "Desktop Client 1").
5.  Click **Create**.

A pop-up will appear showing your Client ID and Client Secret. You can now download the file.

---
### **Step 5: Download the `credentials.json` File**

1.  In the pop-up window from the previous step, click the **DOWNLOAD JSON** button.
2.  A file will be downloaded. **Rename this file to `credentials.json`**.
3.  Place this `credentials.json` file in the same directory where you will run the application executable. The application is coded to look for this specific file name in its root folder.