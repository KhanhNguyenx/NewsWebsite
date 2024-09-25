setInterval(function() {
        $.ajax({
            url: '/home',
            type: 'GET',
            success: function (data) {
                $('.weather-status').html($(data).find('.weather-status').html());
            }
        })
    }, 60000); // Làm mới mỗi 60 giây