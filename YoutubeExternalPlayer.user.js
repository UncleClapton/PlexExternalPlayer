// ==UserScript==
// @name        Youtube External Player
// @namespace   https://github.com/UncleClapton/PlexExternalPlayer
// @description Play youtube videos externally though the PlexExternalPlayerAgent
// @version     1.1
// @author      UncleClapton, Kayomani
// @include     *.youtube.com/*
// @require     http://code.jquery.com/jquery-1.11.3.min.js
// @grant       GM_xmlhttpRequest
// ==/UserScript==

var DEBUG = false;

var makeRequest = function(url){
  if(DEBUG) logMessage("Request Made. Url: " + url);
  return new Promise( function (resolve, reject) {
    GM_xmlhttpRequest({
      method: "GET",
      url: url,
      onload: resolve,
      onerror: reject
    });
  });    
};

var logMessage = function(msg){
  console.log('YoutubeExternal DEBUG: ' + msg);   
};

var pushItemToAgent = function(url, title, time) {
  if(DEBUG) logMessage('Playing ' + url);
  var agentUrl = 'http://localhost:7251/?protocol=1001&url=' + encodeURIComponent(url) +
                 '&title=' + encodeURIComponent(title) +
                 '&time=' + encodeURI(time);

  return new Promise(function (resolve, reject) {
    makeRequest(agentUrl);
  });
};

var pageHookActive = false;
var hookPage = function() {
  $('video').get(0).pause();
  document.getElementById('launch-external').classList.add("yt-uix-button-toggled");
  document.getElementById('player').style.display = "none";
  document.getElementById('placeholder-player').style.display = "none";
  document.getElementById('watch7-sidebar').style.top = "0px";
  document.getElementById('watch7-sidebar').style.marginTop = "0";
  pageHookActive = true;
};
var releasePage = function() {
  document.getElementById('launch-external').classList.remove("yt-uix-button-toggled");
  document.getElementById('player').style.display = "";
  document.getElementById('placeholder-player').style.display = "";
  document.getElementById('watch7-sidebar').style.top = "";
  document.getElementById('watch7-sidebar').style.marginTop = "";
  pageHookActive = false;
};
var clickListener = function() {
  if(pageHookActive) {
    releasePage();
  } else {
    hookPage();
    pushItemToAgent($("#watch7-content link[itemprop='url']").attr("href"),
                    $("#watch7-content meta[itemprop='name']").attr("content"),
                    Math.floor($('video').get(0).currentTime));
  }
};

var bindClicks = function() {
  $('#watch8-secondary-actions').each(function(i, e) {
    e = $(e);
    if (e.find("#launch-external").length <= 0) {
      if(DEBUG) logMessage("Adding Button");
      var buttonTemplate = $('<button class="yt-uix-button yt-uix-button-size-default yt-uix-button-opacity yt-uix-button-has-icon no-icon-markup pause-resume-autoplay action-panel-trigger action-panel-trigger-share yt-uix-tooltip" data-tooltip-title="Play Externally" data-tooltip="Play Externally" title="Play Externally" type="button" id="launch-external"><span class="yt-uix-button-content" id="launchexternal">Play Externally</span></button>');
      buttonTemplate.click(clickListener);
      e.append(buttonTemplate);
    }
  });
};

if(DEBUG) logMessage("WARNING: YoutubeExternal's Debug Mode is Active!");
// Bind buttons and check for new ones every 100ms
setInterval(bindClicks, 100);
bindClicks();