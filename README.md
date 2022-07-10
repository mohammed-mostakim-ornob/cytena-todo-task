# Todo REST API Service

## Deployment Requirements
- Docker

## How To Run
1. Start the docker application.
2. Navigate to the solution directory.
3. Execute the following command.

```bash
docker-compose up
```
The command will start the service and the required `PostgreSQL` database. On the successful start of service, it will be available on the following URL.

```bash
http://localhost:80
```

## Description
The service provides `4` endpoints for `Creating` and `Retrieving` Todo resources.

| No.  | Endpoint | Method | Description |
| ------------- | ------------- | ------------- | ------------- |
| **1**  | `/api/todos`  | **GET**  | Returns the existing Todos. This endpoint paginates the resources.  |
| **2**  | `/api/todos/{id}`  | **GET**  | Returns the Todo with the requested `id`.  |
| **3**  | `/api/todos`  | **POST**  | Adds new Todo.  |
| **4**  | `/api/todos/{id}/image`  | **GET**  | Returns the Image of the Todo with the requested `id`.  |

The endpoints' detailed `OpenAPI` specifications can be found on the following URL.

```bash
http://localhost:80/swagger/index.html
```

## Testing
The solution includes the following testing suites. Tests use an in-memory database and thus can be executed in isolation.

1. Service `Unit Tests`.
2. Database and File Service `Integration Tests`.
3. REST API `Integration Tests`.