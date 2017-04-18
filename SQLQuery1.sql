var markers = @Html.Raw(ViewBag.Jsonlist);
        var patrollers =@Html.Raw(ViewBag.Patrollistjson);
        var days = @Html.Raw(ViewBag.Daysjson);
        var dayschron = days.reverse();
        var colors = ["red","blue","yellow","green","orange"];
        var infowindow = new google.maps.InfoWindow();
        var gmarkers = [];
        var vmarkers=[];
        var dateselect = document.getElementById("dateselect");
        var dateselected = dateselect.value;



 //data for dateselector
                     
            document.getElementById("datetitle").innerHTML="number of scandays:  " + dayschron.length;
        
            for (i=0;i<dayschron.length;i++) {

                var dayformatted = moment(dayschron[i]).format("DD/MM/YYYY");
                var datenode = document.createElement("li");
                var datetext = document.createTextNode( dayformatted);
                datenode.appendChild(datetext);
                document.getElementById("dates-list").appendChild(datenode);
            

            };
            

            for (i=0;i<dayschron.length;i++) {
                var dayformat1 = moment(dayschron[i]).format("DD/MM/YYYY");
                console.log(dayformat1)
                $("#dateselect").append("<option value=" + dayformat1 +">" +  dayformat1 + "</option>")

            };
                

            $('#dateselect').on('change',function() {

                dateselected = this.value;
                vmarkers=[];

                for (i=0; i<markers.length; i++) {

                    marker = gmarkers[i];
                    
                   
                    if(marker.category == dateselected  || dateselected.length == 0  ) {
                        marker.setVisible(true);
                        vmarkers.push(marker);
                    }

                                else{ marker.setVisible(false);
                                }
                }

                showVisibleMarkers(vmarkers);
                markerCluster.clearMarkers();
                markerCluster.addMarkers(vmarkers);
                
                        })
        
           
       
        //make map and add marker 
        function makemap()

        {
            var mapOptions =    {
                center: new google.maps.LatLng(51.20858,3.227961),
                zoom: 14,
                mapTypeId: google.maps.MapTypeId.HYBRID
            };
            
            map = new google.maps.Map(document.getElementById("map"), mapOptions);
                     

            for (i = 0; i < markers.length; i++) {
                addMarker(markers[i]);}
                       

            for (i = 0; i < markers.length; i++) {
                                  
               
                for (j=0;j< (markers.length-i);j++) {
                    var pos = {lat : markers[i].HTQU_Latitude, lng : markers[i].HTQU_Longitude};

                    var posnext =  {lat : markers[j].HTQU_Latitude, lng : markers[j].HTQU_Longitude};
                   
                    if (pos == posnext) {
                        var a = 360.0 / markers.length;
                        var newLat = pos.lat() + -.00004 * Math.cos((+a*i) / 180 * Math.PI);  //x
                        var newLng = pos.lng() + -.00004 * Math.sin((+a*i) / 180 * Math.PI);  //Y
                        var latLng = new google.maps.LatLng(newLat,newLng);
                    }


                }

              
            };

        }

                
        function addMarker(marker) {

            var category = moment(marker.HTQU_CreatedOn).format("DD/MM/YYYY");
            var pos = new google.maps.LatLng(marker.HTQU_Latitude,marker.HTQU_Longitude);


            var datetimestring = moment(marker.HTQU_CreatedOn).format("MMMM Do YYYY, h:mm:ss a");

            var title = datetimestring;
            var content = datetimestring;

            var iconcolor = colors[patrollers.indexOf(marker.HTQU_PatrollerMOBI_ID)]


            marker = new google.maps.Marker({

                title : title,
                position : pos,
                category : category,

                map: map,
                animation : google.maps.Animation.DROP,
                icon : 'http://www.google.com/intl/en_us/mapfiles/ms/micons/' + iconcolor + '-dot.png',


            });

            gmarkers.push(marker);

            //Marker click listener
            google.maps.event.addListener(marker, 'click', function () {

                infowindow.setOptions({
                    content : this.title,
                                })

                infowindow.open(map, marker);

                var posstring = String(pos);


                console.log(posstring)
                document.getElementById("infotext").innerHTML="<b>Data selected marker</b><br><br><u>Date and time : </u><br>"+ title + "<br><br> <u>Coordinates : </u><br>" + posstring.match(/[^()]+/)
                 })

        };
        function showVisibleMarkers(fmarkers) {
            var bounds = map.getBounds();
            count = 0;

            for (var i=0; i< fmarkers.length;i++) {

                var marker = fmarkers[i];
                if(bounds.contains(marker.getPosition())===true) {
                    count++

                }
            }
            $("#statistic").html(count + " reads in actual zoom");

        }

        

        makemap();
        google.maps.event.addListener(map,'idle',function(){
            showVisibleMarkers(gmarkers);

        })
   
        var markerCluster = new MarkerClusterer(map, gmarkers,
            {imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m'});
        
        var countday = []
        //code for chart
