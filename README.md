# Google Docs Batch Markdown Exporter

An interactive command-line tool to navigate a Google Drive account and batch export Google Docs to local Markdown files.

## Features

*   **Interactive REPL**: A shell-like interface to interact with your Google Drive.
*   **Navigation**: Use `ls`, `cd`, and `pwd` to navigate your folder structure.
*   **Tab Completion**: Auto-complete commands and file/folder names with the `Tab` key.
*   **Secure Authentication**: Authorizes with your Google account using a secure, console-based OAuth 2.0 flow.
*   **Markdown Conversion**: Downloads Google Docs and converts them to Markdown, adding a YAML front matter block with:
    *   `title`
    *   `gdrive_id`
    *   `created_time`
    *   `modified_time`
*   **Batch Downloading**:
    *   Download a single file: `download "My Document"`
    *   Download an entire folder and its contents recursively: `download "My Folder"`
    *   Download all items in the current directory: `download .`
*   **Focused**: The tool is hyper-focused on Google Docs and folders, filtering out all other file types (Sheets, Slides, etc.).

## Setup

1.  **Prerequisites**:
    *   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

2.  **Google OAuth Setup**:
    *   Follow the official Google guide to [create a project and enable the Google Drive API](https://developers.google.com/drive/api/v3/quickstart/dotnet#step_1_turn_on_the_api_and_create_credentials).
    *   When creating credentials, choose **"Desktop app"** as the application type.
    *   After creating the OAuth 2.0 Client ID, click **"DOWNLOAD JSON"**.
    *   Rename the downloaded file to `client_secrets.json`.

3.  **Configure the Application**:
    *   Clone this repository.
    *   Place the `client_secrets.json` file inside the `src/gdbme/` directory.
    *   The project is configured to automatically copy this file to the build output directory.

## Usage

1.  **Run the application** from the root of the repository:
    ```sh
    dotnet run --project src/gdbme
    ```

2.  **First-time Authentication**:
    *   On the first run, the application will display a URL.
    *   Copy and paste this URL into your web browser.
    *   Log in to your Google account and grant the application permission.
    *   You will be given an authorization code. Copy it.
    *   Paste the code back into the terminal and press Enter.
    *   Your credentials will be stored locally and securely for future sessions.

3.  **Available Commands**:

| Command                      | Description                                                                                             |
| ---------------------------- | ------------------------------------------------------------------------------------------------------- |
| `ls`                         | List files and folders in the current directory.                                                        |
| `cd <folder>`                | Change directory. Use `..` to go up, `/` for root.                                                      |
| `pwd`                        | Print the current working directory path.                                                               |
| `download <item>`            | Download a file or folder. Use `.` to download everything in the current directory.                     |
| `download <item> -f`         | Force download, overwriting existing files.                                                             |
| `clear`                      | Clear the console screen.                                                                               |
| `exit`                       | Exit the application.                                                                                   |

