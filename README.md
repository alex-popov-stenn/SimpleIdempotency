# Simple Idempotency

This repostiory contains a very simple implementation of idempotency in API.

The idea is to store the request and response of the API in a database and check if the request has been made before. If it has been made before, the response is returned from the database.