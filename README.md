# WavProcessor

WavProcessor is a tool designed for DJs who encounter issues with certain WAV files not playing on CDJs due to the 0xFFFE header. 
This application processes WAV files to correct this header, ensuring compatibility with a wide range of audio equipment.

The bitrate and the sample rate aren't touched at all. You'll be able to play the full audio quality without ANY loss!

## How to Use

1. **Download and Run**: Download the executable from the [Releases](https://github.com/BERNARDO31P/WavProcessor/releases/tag/1.0.2) page and run it.
2. **Select Folder**: Click the "Select Folder" button and choose the directory containing your WAV files.
3. **Processing**: The application will process all WAV files in the selected directory and its subdirectories.
4. **Completion**: Once processing is complete, a status message will indicate that the task is finished.

### Direct downloads
- [Windows](https://github.com/BERNARDO31P/WavProcessor/releases/download/1.0.2/WavProcessor-Portable-x64.exe)
- [MacOS](https://github.com/BERNARDO31P/WavProcessor/releases/download/1.0.2/WavProcessor-Avalonia-Portable-x64.exe) [with Wine](https://www.winehq.org/)

And if none of the above work, here's a Terminal version of the program: 
[TerminalUI](https://github.com/BERNARDO31P/WavProcessor/releases/download/1.0.2/WavProcessor-TUI-Portable-x64-Console.exe)

## Building from Source

1. **Clone the Repository**:
    ```bash
    git clone https://github.com/yourusername/wav-processor.git
    cd wav-processor
    ```

2. **Open the Project**:
   Open the project in Visual Studio.

3. **Build the Project**:
   Build the solution to generate the executable.
