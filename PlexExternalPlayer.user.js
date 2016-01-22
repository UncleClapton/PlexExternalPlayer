// ==UserScript==
// @name         Plex External Player
// @namespace    https://github.com/UncleClapton/PlexExternalPlayer
// @version      1.3.1
// @description  Play plex videos in an external player
// @author       Kayomani
// @include     /^https?://.*:32400/web.*
// @include     http://*:32400/web/index.html*
// @require     http://code.jquery.com/jquery-1.11.3.min.js
// @grant       GM_xmlhttpRequest
// ==/UserScript==

var makeRequest = function(url){
   return new Promise( function (resolve, reject) {
       GM_xmlhttpRequest({
           method: "GET",
            headers: {
                "X-Plex-Token":localStorage["myPlexAccessToken"]
            },
           url: url,
           onload: resolve,
           onerror: reject
       });
   });    
};

var logMessage = function(msg){
    console.log('Plex External: ' + msg);   
};

var markAsPlayedInPlex = function(id) {
    logMessage('Marking ' + id + ' as played');
    return makeRequest(window.location.origin + '/:/scrobble?key='+ id +'&identifier=com.plexapp.plugins.library');
};

var pushItemToAgent = function(url, id, grandparentTitle, title, rating, filePath) {
    
    logMessage('Playing ' + url);
    var agentUrl = 'http://localhost:7251/?protocol=1202&url=' + encodeURIComponent(url) + "&id=" + id + "&title=" + title + "&grandparentTitle=" + grandparentTitle + "&rating=" + rating + "&filePath=\"" + filePath + "\"";
  
     return new Promise(function (resolve, reject) {
         makeRequest(agentUrl).then(function(){
             markAsPlayedInPlex(id).then(resolve, reject);
         },reject);
     });
};

var clickListener = function(e) {
    e.preventDefault();
    e.stopPropagation();
    var a = jQuery(e.target).closest('a');
    var link = a.attr('href');
    var openFolder = a.attr('data-type') === 'folder';
  
    var url = link;
    if (link === '#' || link === undefined) {
        url = window.location.hash;
    }

    if (url.indexOf('%2Fmetadata%2F') > -1) {
        var idx = url.indexOf('%2Fmetadata%2F');
        var id = url.substr(idx + 14);

        // Get metadata
        makeRequest(window.location.origin + '/library/metadata/' + id + '?checkFiles=1&includeExtras=1')
        .then(function(response){
             // Play the first availible part
             var parts = response.responseXML.getElementsByTagName('Part');
             var video = response.responseXML.getElementsByTagName('Video');
             var videoMetadata = video[0];
                for (var i = 0; i < parts.length; i++) {
                    if (parts[i].attributes['key'] !== undefined) {
                        var url = window.location.protocol + '//' + window.location.host + parts[i].attributes['key'].value;
                        var grandparentTitle = "";
                        var title = "";
                        var rating = "";
                        var filePath = "";

                        if (videoMetadata !== undefined) {
                            if(videoMetadata.attributes['grandparentTitle'] !== undefined)
                                grandparentTitle = videoMetadata.attributes['grandparentTitle'];
                            if(videoMetadata.attributes['title'] !== undefined)
                                title = videoMetadata.attributes['title'];
                            if(videoMetadata.attributes['contentRating'] !== undefined)
                                rating = videoMetadata.attributes['contentRating'];
                            if(parts[i].attributes['file'] !== undefined)
                                filePath = parts[i].attributes['file'];
                        }

                        pushItemToAgent(url, id, grandparentTitle, title, rating, filePath);

                        return;
                    }
                }
        });
    }
};

var bindClicks = function() {
    jQuery(".glyphicon.play").each(function(i, e) {
        e = jQuery(e);
        if (!e.hasClass('plexextplayer')) {
            if (!e.parent().hasClass('hidden')) {
                e.addClass('plexextplayer');
                var parent = e.parent().parent();
                if (parent.is('div') && parent.hasClass('media-poster-actions')) {
                    var template = jQuery('<button class="play-btn media-poster-btn btn-link" tabindex="-1"><i class="glyphicon play plexextplayer plexextplayerico"></i></button>');
                    parent.prepend(template);
                    template.click(clickListener);
                }
            }
        }
    });
};

// Make buttons smaller
jQuery('body').append('<style>.media-poster-btn { padding: 8px !important; } .glyphicon.plexfolderextplayerico:before {  content: "\\e343";   } .glyphicon.plexextplayerico:before {  content: "\\e161";   }</style>');

// Bind buttons and check for new ones every 100ms
setInterval(bindClicks, 100);
bindClicks();