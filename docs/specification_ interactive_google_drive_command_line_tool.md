# **Specification: Interactive Google Drive Command-Line Tool**

## **1\. Overview**

This document specifies a self-contained, interactive command-line interface (CLI) application for navigating and downloading content from a user's Google Drive. The primary purpose of the tool is to provide a file-system-like experience for interacting with Google Drive folders and documents, with a specific feature to convert and download Google Docs as Markdown files with prepended metadata.

## **2\. Core Features**

### **2.1. Interactive Shell Interface**

* The application shall operate as a persistent, interactive shell after launch.
* It shall display a prompt indicating the user's current virtual path within their Google Drive (e.g., /My Drive/Projects\>).
* The shell shall accept user-inputted commands to perform various actions.

### **2.2. Google Drive Navigation**

* The tool shall maintain a state representing the user's "current working directory" within Google Drive.
* Users shall be able to list the contents (files and folders) of their current directory.
* Users shall be able to change their current directory to a sub-folder or a parent folder.

### **2.3. File & Folder Downloading**

* The application shall provide a command to download content from Google Drive to the local file system.
* Users can initiate a download for a single Google Doc.
* Users can initiate a recursive download for an entire Google Drive folder and its sub-contents.
* The application shall provide progress indicators for downloads, especially for large files or recursive folder downloads.
* By default, the download command will not overwrite existing files. A force flag (`-f`) must be used to overwrite.

### **2.4. Markdown Conversion & Metadata**

* When a Google Doc is downloaded, it must be exported directly from the Google Drive API using the `text/markdown` MIME type.
* The downloaded Markdown file must be prepended with a YAML front matter block.
* This front matter shall contain the following metadata from the original Google Doc:
  * title: The original name of the document.
  * created: The creation timestamp in ISO 8601 format.
  * modified: The last modified timestamp in ISO 8601 format.
  * gdrive\_id: The unique file ID from Google Drive.

### **2.5. Authentication**

* The application shall use the OAuth 2.0 protocol to gain authorized, read-only access to the user's Google Drive.
* On first run, the application will guide the user through a browser-based authentication flow to grant permissions.
* Subsequent runs shall use a stored token for authentication. The application will attempt to use the operating system's secure credential store (e.g., Windows Credential Manager, macOS Keychain). If a system store is unavailable, the token will be encrypted and stored in a file within a user-specific application data directory.

## **3\. User Commands**

The following commands shall be supported by the interactive shell:

| Command | Arguments | Description |
| :---- | :---- | :---- |
| ls | (none) | Lists the files and folders in the current Google Drive directory. Folders will be displayed in blue, and files in the default terminal color. |
| cd | \<folder\_name\> | Changes the current directory to the specified sub-folder. |
|  | .. | Navigates to the parent directory. |
|  | / | Navigates to the root "My Drive" directory. |
| pwd | (none) | Prints the full path of the current working directory. |
| download | `[-f]` \<item\_name\> | Downloads the specified file or folder to the local directory where the app is running. Supports names with spaces if quoted. The optional `-f` flag will force an overwrite of existing local files. |
|  | `[-f]` . | Downloads the entire contents of the current Google Drive directory. The optional `-f` flag will force an overwrite of existing local files. |
| help | (none) | Displays a summary of all available commands and their usage. |
| clear / cls | (none) | Clears the terminal screen. |
| exit / quit | (none) | Terminates the application. |

## **4\. Non-Functional Requirements**

### **4.1. Error Handling & API Interaction**

*   The application must handle errors gracefully without crashing. User-facing errors (e.g., invalid command, file not found) and internal errors (e.g., network issues, API failures) shall be reported clearly to the user in the shell.
*   The application must implement an exponential backoff strategy when making Google Drive API calls to respect API rate limits and handle transient network errors.

### **4.2. Packaging**

*   The application must be distributable as a single, self-contained executable file for both Windows x64 and macOS arm64, leveraging .NET Native AOT compilation. It should not require the end-user to install any separate runtimes or dependencies like .NET.

### **4.3. Versioning**

*   The application will use the MinVer dotnet tool to calculate versions. The version will use the semantic 0.0.0 format.

### **4.4. CI/CD**

*   The application will have CI/CD scripts for testing and deployment. The app will be deployed as an artifact attached to the release notes.

### **4.5. Permissions**

*   The application shall only request read-only permissions (`drive.readonly`) to the user's Google Drive to ensure data safety. The credential information will be embedded in the application, with secrets being stored as repository secrets in GitHub and injected as part of the CD process, or via a local git ignored file for development purposes.

## **5\. Glossary of Terms**

| Term | Definition |
| :---- | :---- |
| **CLI** | Command-Line Interface. A text-based user interface used to run programs, manage computer files, and interact with the computer. |
| **OAuth 2.0** | An industry-standard protocol for authorization. It allows applications to obtain limited access to user accounts on an HTTP service. |
| **Markdown** | A lightweight markup language for creating formatted text using a plain-text editor. |
| **YAML Front Matter** | A block of key-value pairs formatted in YAML, placed at the beginning of a text file to store metadata. |
| **Self-Contained Executable** | A single application file that includes all necessary dependencies and runtimes, allowing it to run on a target system without prior installation of a framework. |
| **ISO 8601** | An international standard for representing dates and times. |

