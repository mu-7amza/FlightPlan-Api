const api_base_url = "http://localhost:3000/api/v1/flightplan/";

let username =  "admin";
let password = "password";

async function get_all_flight_plans() {
    let api_response = await fetch(api_base_url,{
        method: "GET",
        headers: {
            'Authorization': 'Basic ' + btoa(username + ":" + password)
        }
    });

    if(api_response.status !== 200){
        alert(api_response.status);
        return;
    }

    let api_data = await api_response.json();   

    let html = '';

    api_data.forEach(flight_plan => {
        html += '<div class="card mb-5">' +
        '       <div class="card-header container-fluid">' +
        '           <div class="row">' +
        '              <div class="col-md-10">' +
        '                   <h5>' + flight_plan.flight_type + ' flight plan from ' + flight_plan.departing_airport + ' to ' + flight_plan.arrival_airport + '</h5>' +
        '               </div>' +
        '               <div class="col-md-2 float-right">' +
        '                  <button class="btn btn-danger" style="margin-left: 1em" ' +
        '                   onclick="delete_flight_plan(\'' + flight_plan.flight_plan_id + '\')">Delete</button>' +
        '              </div>' +
        '           </div>' +
        '    </div>' +
        '    <div class="card-body">' +
        '        <div class="row m-3 mb-4">' +
        '            <div class="col-3">' +
        '                <h6>Flight Type:</h6> '+ flight_plan.flight_type +
        '            </div>' +
        '            <div class="col-3">' +
        '                <h6>Aircraft Identification:</h6>' + flight_plan.aircraft_identification +
        '            </div>' +
        '            <div class="col-3">' +
        '                <h6>Aircraft Type:</h6> ' + flight_plan.aircraft_type +
        '            </div>' +
        '            <div class="col-3">' +
        '                <h6>Fuel on Board:</h6>' + flight_plan.fuel_hours + ' hours ' + flight_plan.fuel_minutes +
        '            </div>' +
        '        </div>' +
        '        <div class="row m-3 mb-4">' +
        '            <div class="col-3">' +
        '                <h6>Filed Altitude:</h6> ' + flight_plan.altitude + ' feet' +
        '            </div>' +
        '            <div class="col-3">' +
        '                <h6>Filed Airspeed:</h6> ' + flight_plan.airspeed + ' knots' +
        '            </div>' +
        '            <div class="col-3">' +
        '                <h6>Departure Time:</h6>' + new Date(flight_plan.departure_time).toLocaleString()  +
        '            </div>' +
        '            <div class="col-3">' +
        '                <h6>Estimated Arrival Time:</h6>' + new Date(flight_plan.estimated_arrival_time).toLocaleString()  +
        '            </div>' +
        '        </div>' +
        '        <div class="row m-3 mb-4">' +
        '            <div class="col-12"><h6>Route: </h6>' + flight_plan.route + '</div>' +
        '        </div>' +
        '        <div class="row m-3 mb-4">' +
        '            <div class="col-12"><h6>Remarks: </h6>'+ flight_plan.remarks + '   </div>' +
        '        </div>' +
        '    </div>' +
        '    <div class="card-footer text-muted"> ' +
        '    Flight Plan Id: ' +   flight_plan.flight_plan_id   +
        '   </div>' +
        '</div>'

     document.getElementById('flight_plan_list').innerHTML = html;});
    }
async function delete_flight_plan (flight_plan_id){
    
    let response = await fetch(api_base_url+flight_plan_id,{
        method:'DELETE',
        headers: {
            'Authorization': 'Basic ' + btoa(username + ":" + password)
        }

    });

    if(response.status !== 200){
        alert(response.status);
    }

}

async function load_flight_plan(){

    if(document.getElementById("flightPlanId").value == ""){
        alert("Please enter flight id first !")
    }else{
        let flight_plan_id = document.getElementById("flightPlanId").value;
        let api_response = await fetch(api_base_url + flight_plan_id, {
            method: 'GET',
            headers: {
                'Authorization': 'Basic ' + btoa(username + ":" + password)
            }
        });
    
        if(api_response.status !== 200) {
            alert(api_response.status);
            return;
        } 
    
    
        let api_data = await api_response.json();
    
        document.getElementById('tailNumber').value = api_data.aircraft_identification;
        document.getElementById('aircraftType').value = api_data.aircraft_type;
        document.getElementById('airspeed').value = api_data.airspeed;
        document.getElementById('altitude').value = api_data.altitude;
        document.getElementById('paxOnBoard').value = api_data.number_onboard;
        document.getElementById('departureTime').value = api_data.departure_time.substring(0, api_data.departure_time.length - 1);
        document.getElementById('arrivalTime').value = api_data.estimated_arrival_time.substring(0, api_data.estimated_arrival_time.length - 1);
        document.getElementById('departAirport').value = api_data.departing_airport;
        document.getElementById('arriveAirport').value = api_data.arrival_airport;
        document.getElementById('route').value = api_data.route;
        document.getElementById('remarks').value = api_data.remarks;
        document.getElementById('fuelHours').value = api_data.fuel_hours;
        document.getElementById('fuelMinutes').value = api_data.fuel_minutes;
        document.getElementById('paxOnBoard').value = api_data.number_onboard;
    
        if (api_data.flight_type === 'VFR') {
            document.getElementById('inlineVFR').checked = true;
        }
        else if (api_data.flight_type === 'IFR') {
            document.getElementById('inlineIFR').checked = true;
        }
    
        document.getElementById('fileButton').hidden = true;
        document.getElementById('updateButton').hidden = false;
        alert("Loaded Succesfuuly");
    }
    
}
async function update_flight_plan() {
    let selected_flight_type = 'Unknown';
    if(document.getElementById('inlineVFR').checked) {
        selected_flight_type = 'VFR';
    }
    else if(document.getElementById('inlineIFR').checked) {
        selected_flight_type = 'IFR';
    }

    let api_response = await fetch(api_base_url, {
        method: "PUT",
        headers: {
            'Authorization': 'Basic ' + btoa(username + ":" + password),
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            flight_plan_id: document.getElementById("flightPlanId").value,
            altitude: Number(document.getElementById('altitude').value),
            aircraft_identification: document.getElementById('tailNumber').value,
            aircraft_type: document.getElementById('aircraftType').value,
            airspeed: Number(document.getElementById('airspeed').value),
            arrival_airport: document.getElementById('arriveAirport').value,
            flight_type: selected_flight_type,
            departing_airport: document.getElementById('departAirport').value,
            departure_time: new Date(document.getElementById('departureTime').value).toISOString(),
            estimated_arrival_time: new Date(document.getElementById('arrivalTime').value).toISOString(),
            route: document.getElementById('route').value,
            remarks: document.getElementById('remarks').value,
            fuel_hours: Number(document.getElementById('fuelHours').value),
            fuel_minutes: Number(document.getElementById('fuelMinutes').value),
            number_onboard: Number(document.getElementById('paxOnBoard').value)
        })
    });

    if(api_response.status !== 200) {
        alert(api_response.status);
    }
    alert("Updated Succesfuuly");

}

async function file_flight_plan() {
    let selected_flight_type = 'Unknown';
    if(document.getElementById('inlineVFR').checked) {
        selected_flight_type = 'VFR';
    }
    else if(document.getElementById('inlineIFR').checked) {
        selected_flight_type = 'IFR';
    }

    let api_response = await fetch(api_base_url + 'file', {
        method: 'POST',
        headers: {
            'Authorization': 'Basic ' + btoa(username + ":" + password),
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            flight_plan_id: document.getElementById("flightPlanId").value,
            altitude: Number(document.getElementById('altitude').value),
            aircraft_identification: document.getElementById('tailNumber').value,
            aircraft_type: document.getElementById('aircraftType').value,
            airspeed: Number(document.getElementById('airspeed').value),
            arrival_airport: document.getElementById('arriveAirport').value,
            flight_type: selected_flight_type,
            departing_airport: document.getElementById('departAirport').value,
            departure_time: new Date(document.getElementById('departureTime').value).toISOString(),
            estimated_arrival_time: new Date(document.getElementById('arrivalTime').value).toISOString(),
            route: document.getElementById('route').value,
            remarks: document.getElementById('remarks').value,
            fuel_hours: Number(document.getElementById('fuelHours').value),
            fuel_minutes: Number(document.getElementById('fuelMinutes').value),
            number_onboard: Number(document.getElementById('paxOnBoard').value)
        })
    });

    if(api_response.status !== 200) {
        alert(api_response.status);
    }
    alert("Stored Succsefully Succesfuuly");

}