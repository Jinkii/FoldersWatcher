# FoldersWatcher
Windows Service Console App to monitor crud operations in one or multiple directories in Windows.

# How to use
- Publish the application
- Start the application directly from the .exe file named FoldersWatcher.exe (inside the Debug folder) or install it as a Windows Service by going to CMD (Command Prompt) as Administrator, type in "cd {foldername}" then "FoldersWatcher.exe install start"   --> to uninstall type in the same directory the command "FoldersWatcher.exe uninstall stop"

# Change folders to watch
Open the file App.config and add a new key with the folder path as value, the program will create a thread automatically and monitor that folder too, you can have as many threads as your system allows.

# Output Log
The events will be written on the log file under the folder "logs" with the name {basedir}\logs\${shortdate}_folderwatcher.log

Inside the log the events will be written as:

- "New file created: Source: {name of the source file} | Event Type: {event type} | Name: {file name} | {time} | Filepath: {file path}"
- "File changed: Source: {name of the source file} | Event Type: {event type} | Name: {file name} | {time} | Filepath: {file path}"
- "File deleted: Source: {name of the source file} | Event Type: {event type} | Name: {file name} | {time} | Filepath: {file path}"
- "File renamed: Source: {name of the source file} | Event Type: {event type} | Name: {file name} | {time} | Filepath: {file path}"
