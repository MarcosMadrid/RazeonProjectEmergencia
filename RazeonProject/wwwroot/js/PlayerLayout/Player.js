var audioPlayer;
let thumbinterval;
let previousVolume;

class Time {
    constructor(hours, minutes, seconds) {
        this.hours = hours || 0;
        this.minutes = minutes || 0;
        this.seconds = seconds || 0;
    }

    static fromSeconds(totalSeconds) {
        const hours = Math.floor(totalSeconds / 3600);
        const minutes = Math.floor((totalSeconds % 3600) / 60);
        const seconds = totalSeconds % 60;
        return new Time(hours, minutes, seconds);
    }

    getTotalSeconds() {
        return this.hours * 3600 + this.minutes * 60 + this.seconds;
    }

    toString() {
        const roundedSeconds = Math.round(this.seconds);
        return `${String(this.hours).padStart(2, '0')}:${String(this.minutes).padStart(2, '0')}:${String(roundedSeconds).padStart(2, '0')}`;
    }
}

function PutEventsOn(audioUrl) {
    if (audioPlayer) {
        audioPlayer.unload();
    }
    audioPlayer = new Howl({
        src: [audioUrl],
        autoplay: true,
        format: ['mp3'],
        html5: true,
        preload: 'auto'
    });

    audioPlayer.on('load', function () {
        handleStateChange('load');
    });
    audioPlayer.on('play', function () {
        handleStateChange('play');
    });
    audioPlayer.on('pause', function () {
        handleStateChange('pause');
    });
    audioPlayer.on('end', function () {
        handleStateChange('end');
    });
    audioPlayer.on('mute', function () {
        handleStateChange('mute', audioPlayer._muted);
    });
    audioPlayer.on('volume', function () {
        handleStateChange('volume', audioPlayer._volume);
    })
    audioPlayer.on('stop', function () {
        console.log("stop");
        handleStateChange('stop');
    })
}


function getCurrentTime() {
    if (!audioPlayer) {
        return 0;
    }
    return Time.fromSeconds(audioPlayer.seek());
}

function getTotalDuration() {
    if (!audioPlayer) {
        return 0;
    }
    return Time.fromSeconds(audioPlayer._duration);
}

function play() {
    if (!audioPlayer) {
        initializePlayer();
    }
    if (audioPlayer.playing()) {
        pause();
        return;
    }
    audioPlayer.play();
}

function pause() {
    if (!audioPlayer) {
        initializePlayer();
    }
    audioPlayer.pause();
}

function mute() {
    if (!audioPlayer) {
        return;
    }
    audioPlayer.mute(!audioPlayer._muted);
}

function volume(value) {
    if (!audioPlayer) {
        return;
    }
    if (!audioPlayer._muted) {
        previousVolume = audioPlayer.volume();
    }
    audioPlayer.volume(value);
}

function moveTimeMusic(value) {
    if (!audioPlayer) {
        return;
    }
    if (!audioPlayer.playing()) {
        audioPlayer.play();
    }
    const position = value * getTotalDuration().getTotalSeconds() / 100;
    audioPlayer.seek(position);
}

async function animateThumb() {
    if (!audioPlayer) {
        return;
    }
    if (audioPlayer.playing()) {
        let currentTime = getCurrentTime().getTotalSeconds();
        const duration = getTotalDuration().getTotalSeconds();

        if (duration > 0) {
            const currentTimeObj = Time.fromSeconds(currentTime);
            const formattedTime = currentTimeObj.toString();

            const newValue = (currentTime / duration) * 100;
            $(".rangePlayerMusic").val(newValue);
            $(".textCurrentTimePlayerMusic").text(formattedTime);
        }
    }
}


function handleStateChange(state, actionValue) {
    if (state == 'load') {
        $(".rangePlayerMusic").prop('disabled', false);
        $(".textEndTimePlayerMusic").text(getTotalDuration().toString());
    }
    if (state == 'play') {
        var btn_player = $(".bi.btn-player.bi-play-circle-fill")
        btn_player.removeClass("bi-play-circle-fill");
        btn_player.addClass("bi-pause-circle-fill");
        thumbInterval = setInterval(animateThumb, 200);
        return;
    }
    if (state == 'pause') {
        var btn_player = $(".bi.btn-player.bi-pause-circle-fill")
        btn_player.removeClass("bi-pause-circle-fill");
        btn_player.addClass("bi-play-circle-fill");
        clearInterval(thumbInterval);
        return;
    }
    if (state == 'mute') {
        if (actionValue) {
            var btn_player_audio = $(".btn_player_audio")
            btn_player_audio.removeClass("bi-volume-down-fill");
            btn_player_audio.removeClass("bi-volume-up-fill");
            btn_player_audio.addClass("bi-volume-mute-fill");
        } else {
            if (previousVolume !== undefined) {
                audioPlayer.volume(previousVolume);
            }
            handleStateChange('volume', audioPlayer._volume);
        }
        return;
    }
    if (state == 'volume') {
        if (audioPlayer._muted) {
            return;
        }
        if (actionValue <= 0) {
            var btn_player_audio = $(".btn_player_audio")
            btn_player_audio.removeClass("bi-volume-down-fill");
            btn_player_audio.addClass("bi-volume-mute-fill");
            return;
        }
        if (actionValue > 0 && actionValue <= 0.49) {
            var btn_player_audio = $(".btn_player_audio");
            btn_player_audio.removeClass("bi-volume-mute-fill");
            btn_player_audio.removeClass("bi-volume-up-fill");
            btn_player_audio.addClass("bi-volume-down-fill");
            return;
        }
        if (actionValue >= 0.50) {
            var btn_player_audio = $(".btn_player_audio")
            btn_player_audio.removeClass("bi-volume-down-fill");
            btn_player_audio.removeClass("bi-volume-mute-fill");
            btn_player_audio.addClass("bi-volume-up-fill");
            return;
        }
        return;
    }
    if (state == 'seek') {
        $(".textCurrentTimePlayerMusic").text(actionValue.toString());
    }
    if (state == 'stop') {
        clearInterval(animateThumb);
    }
}

function toggleFullScreen() {
    var playerFullScreen = document.getElementById("fullScreenPlayer");

    if (!document.fullscreenElement && !document.mozFullScreenElement && !document.webkitFullscreenElement && !document.msFullscreenElement) {
        // If no element is in full-screen mode, enter full-screen mode
        if (playerFullScreen.requestFullscreen) {
            playerFullScreen.requestFullscreen().catch(err => {
                console.log(`Error attempting to enable full-screen mode: ${err.message}`);
            });
        } else if (playerFullScreen.mozRequestFullScreen) { // Firefox
            playerFullScreen.mozRequestFullScreen().catch(err => {
                console.log(`Error attempting to enable full-screen mode: ${err.message}`);
            });
        } else if (playerFullScreen.webkitRequestFullscreen) { // Chrome, Safari and Opera
            playerFullScreen.webkitRequestFullscreen().catch(err => {
                console.log(`Error attempting to enable full-screen mode: ${err.message}`);
            });
        } else if (playerFullScreen.msRequestFullscreen) { // IE/Edge
            playerFullScreen.msRequestFullscreen().catch(err => {
                console.log(`Error attempting to enable full-screen mode: ${err.message}`);
            });
        }
    } else {

        if (document.exitFullscreen) {
            document.exitFullscreen().catch(err => {
                console.log(`Error attempting to exit full-screen mode: ${err.message}`);
            });
        } else if (document.mozCancelFullScreen) { // Firefox
            document.mozCancelFullScreen().catch(err => {
                console.log(`Error attempting to exit full-screen mode: ${err.message}`);
            });
        } else if (document.webkitExitFullscreen) { // Chrome, Safari and Opera
            document.webkitExitFullscreen().catch(err => {
                console.log(`Error attempting to exit full-screen mode: ${err.message}`);
            });
        } else if (document.msExitFullscreen) { // IE/Edge
            document.msExitFullscreen().catch(err => {
                console.log(`Error attempting to exit full-screen mode: ${err.message}`);
            });
        }
    }
}
$(document).on('webkitfullscreenchange mozfullscreenchange fullscreenchange', function (e) {
    var playerFullScreen = $("#fullScreenPlayer");
    if (!playerFullScreen.hasClass('d-none')) {
        playerFullScreen.addClass("d-none");
    } else {
        playerFullScreen.removeClass("d-none");
    }
});

function ClickedCardMusic(itemId, itemType) {
    if (itemType == "Track") {
        fetch('/Provider/LoadSong?idTrack=' + itemId)
            .then(response => response.blob())
            .then(blob => {
                var audioUrl = URL.createObjectURL(blob);
                PutEventsOn(audioUrl);
            })
            .catch(error => {
                console.error("Error fetching track:", error);
            });

        $.ajax({
            url: '/Provider/LoadSongImg?idTrack=' + itemId,
            type: 'GET',
            success: function (response) {
                var imageUrl = window.location.origin + '/Provider/LoadSongImg?idTrack=' + itemId; 
                $("#fullScreenPlayer").css("background-image", 'url(' + imageUrl + ')');
                $("#fullScreenPlayer img").attr("src", imageUrl);
                $("#playerDiv img").attr("src", imageUrl);
            },
            error: function (xhr, status, error) {
                console.error("Error fetching image:", error);
            }
        });

        $.ajax({
            url: '/Provider/GetSongLoaded?idTrack=' + itemId,
            type: 'GET',
            success: function (response) {
                $("#playerArtistData").text();
                $(".playerMusicTitle")[0].innerHTML = response.title;
                $(".playerMusicTitle")[1].innerHTML = response.title;
            },
            error: function (xhr, status, error) {
                console.error("Error fetching image:", error);
            }
        });
    }

    if (itemType == "Album") {
        fetch('/Provider/LoadAlbumSongs?idAlbum=' + itemId)
            .then(response => response.json())
            .then(data => {
                const audioUrls = data.map(track => URL.createObjectURL(track));
                PutEventsOn(audioUrls);
            })
            .catch(error => {
                console.error("Error fetching tracks:", error);
            });
    }

}

