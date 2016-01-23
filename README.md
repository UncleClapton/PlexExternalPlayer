# External plex player

This is my personal fork of the External Plex player, which will work with remote plex servers as well as local ones using the plex-hosted media stream.

The original project can be found [here](https://github.com/Kayomani/PlexExternalPlayer)

External Plex Player is a plex modification that allows for browsing using the web interface but then opting to play files in your standard media player.

![screenshot](http://i.imgur.com/aM37t76.png "screenshot")

### Installation
1. Install Chrome or Firefox
2. Install [TamperMonkey](https://chrome.google.com/webstore/detail/tampermonkey/dhdgffkkebhmkfjojejmpbldmpobfkfo?hl=en) (Chrome) or [Greasemonkey](https://addons.mozilla.org/en-US/firefox/addon/greasemonkey/) (Firefox).
3. Extract the agent somewhere where it won't be moved from.
4. Follow the readme.txt on how to set up the agent with your media player of choice.
5. Run it at startup using a shortcut in the startup folder.
6. Run the agent manually so you get the icon in the tray. Requires .Net 4.5 on Windows, may work on Linux/mono.
7. Install the script: [Click Me!](https://raw.githubusercontent.com/UncleClapton/PlexExternalPlayer/clapton/master/PlexExternalPlayer.user.js)*
8. If you wish to install the Youtube Script: [Click Me!](https://raw.githubusercontent.com/UncleClapton/PlexExternalPlayer/clapton/master/YoutubeExternalPlayer.user.js)*
9. Refresh any existing plex pages you have open.

* If the install dialog does not immediately appear for you in Firefox, a banner with the option to install the script should appear after the script loads.

## Documentation 1.4+
**Note: ** These features are currently in development, and may change in the future.

### Generic Protocol
* Designed to be an open protocol for other scripts to send files to the agent.
* It is currently disabled by default, however it can be enabled by switching "EnableGenericProtocol" in the agent .exe.config file to `true`.
* Format: `http://localhost:7251/?protocol=1001&url=(url)&title=(title)`


### Player Argument variables
The PlayerPlexArguments and PlayerGenericArguments property accepts metadata variables to pass into

#####PlayerPlexArguments

| Variable | Description |
|----------|-------------|
|%url%|The hosted file url|
|%fileId%|The plex-assigned file ID|
|%title%|The display title of the media file. Typically the movie name, or episode title.|
|%seriesTitle%| The series name of the media file. If the media file is a movie, or has no series name, this will contain blank string.|
|%fullTitle%|A combined variable for "title" and "seriesTitle". If no series title is available, this will only contain the  "title"|
|%contentRating%|The content rating of the media file. |
|%filePath%|The server's local storage path for the media file.|

#####PlayerGenericArguments

| Variable | Description |
|----------|-------------|
|%url%|The hosted file url|
|%title%|The title of the video stream|
