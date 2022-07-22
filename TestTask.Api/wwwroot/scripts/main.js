var markersData; // храним данные маркеров

start();

// проводим авторизацию пользователя и получаем данные маркеров
async function start () {  
    if (sessionStorage.getItem("accessToken") === null)
        window.location.replace("/login.html");
    
    var response = await fetch("api/markers", {
        method: "GET",
        headers: {
            "Accept": "*/*",
            "Content-Type": "application/json",
            "Authorization": 'Bearer ' + sessionStorage.getItem("accessToken")
        }
    })
    if (response.status === 401)
        window.location.replace("/login.html");

    markersData = JSON.parse(await response.text());
}

var myMap;
ymaps.ready(init);

function init () { // инициализация карты и расставление маркеров
    
    var headers = [ // Заголовки. Массив массивов символов HEX формата
        [0x5a, 0x65, 0x72, 0x6f],
        [0x4f, 0x6e, 0x65],
        [0x54, 0x77, 0x6f],
        [0x54, 0x68, 0x72, 0x65, 0x65],
        [0x46, 0x6f, 0x75, 0x72],
        [0x46, 0x69, 0x76, 0x65],
        [0x53, 0x69, 0x78],
        [0x53, 0x65, 0x76, 0x65, 0x6e],
        [0x45, 0x69, 0x67, 0x68, 0x74],
        [0x4e, 0x69, 0x6e, 0x65]]


    myMap = new ymaps.Map('map', {
        center: [55.76, 37.64], // Москва
        zoom: 11
    }, {
        searchControlProvider: 'yandex#search'
    });
    
    var myGeoObjects;
    
    for (var markerId in markersData) // расставляем маркеры на карте
    {
        myGeoObjects = myMap.geoObjects
            .add(new ymaps.Placemark([parseFloat(markersData[markerId]["latitude"]), parseFloat(markersData[markerId]["longitude"])], {
                balloonContentHeader: hexArrayToUnicodeString(headers[Math.floor(Math.random() * 10)]),
                balloonContentBody: markersData[markerId]["markerText"]
            }, {
                preset: 'islands#icon',
                iconColor: '#47AB11'
            }))
    }

    myGeoObjects.events.add('click', function (e) { // ловим событие клика по маркеру
        // Получение координат щелчка
        var thisPlacemark = e.get('target');
        
        // при каждом клике меняю заголовок всплявающего окна
        thisPlacemark.properties.set('balloonContentHeader', hexArrayToUnicodeString(headers[Math.floor(Math.random() * 10)]));
    });
        
    document.getElementById('submitLogout').onclick = function () { // елси нажали кнопку Logout
        fetch("api/logout", {
            method: "POST",
            headers: {
                "Accept": "*/*",
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                refreshToken: sessionStorage.getItem("refreshToken")
            })
        });
        sessionStorage.clear();
        window.location.replace("/login.html");
    }
}

function hexArrayToUnicodeString(header)
{
    var s = ''
    
    for (var b in header) {
        s += String.fromCharCode(header[b])
    }
    return s;
}