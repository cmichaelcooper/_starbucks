EquipmentFunctionality: function (equipment) {
    $('#filter-toggle').on('click', function () {
        $(this).next('#filters-holder').slideToggle(250);
    });

    for (var item in equipment) {
        var div = $('#product-v2').clone().removeAttr('id').removeAttr('style');
        div.find('img').attr('title', equipment[item].Title);
        div.find('h3').text(equipment[item].Title);
        div.find('p').text(equipment[item].Description);

        var link = div.find('a');
        if (equipment[item].Video) {
            link.eq(0).data("reveal-id", "videoModal_" + item);
            link.eq(1).attr('href', "http://player.vimeo.com/video/" + equipment[item].Video);

            var m = div.find('.trainingVideoModal');
            m.attr('id', "videoModal_" + item);
            m.find('iframe').attr({
                'id': "videoModal_" + item,
                'src': '//player.vimeo.com/video/' + equipment[item].Video + '?api=1&player_id=video-player_' + item
            });
        }
        else {
            link.attr('href', equipment[item].URL);
        }

        var img = div.find('img');
        if (equipment[item].Image) {
            img.attr('src', '/img/equipment/200/' + equipment[item].Image + '.jpg');
        }
        else {
            img.attr('src', '/img/icon-play-video.png');
        }

        $('#equipment-list').append(div);
    }
}