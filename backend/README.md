# Linked Out Backend

Modeled as a RESTful API

Go to http://localhost:5048/swagger/index.html to inspect the endpoints this API provides, as well as the the requests it accepts.

All requests besides login and registration need to be authenticated with a valid token (received through login or registration).

Tokens last for 4 hours.

Certain requests require certain levels of Authorisation. They are annotated as able to return the status code 403 Forbidden

For HTTPS requests, use the following link: https://localhost:8043


