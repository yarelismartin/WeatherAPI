# WeatherAPI

## Table of Contents
- [Setup Instructions](#setup-instructions)
  - [Models](#models)
    - [User](#user)
    - [Location](#location)
- [API Documentation](#api-documentation)
    - [POST /api/auth/register](#post-apiauthregister)
    - [POST /api/auth/login](#post-apiauthlogin)
    - [GET /api/weather/{location}](#get-apiweatherlocation)
    - [GET /api/weather/forecast/{location}](#get-apiweatherforecastlocation)
    - [POST /api/favorites](#post-apifavorites)
    - [GET /api/favorites](#get-apifavorites)
    - [DELETE /api/favorites/{id}](#delete-apifavoritesid)
- [Architecture Decisions](#architecture-decisions)
- [Running Tests](#running-tests)

---

## Setup Instructions
#### For this project to run successfully, you'll need the following:

- [Visual Studio](https://visualstudio.microsoft.com/downloads/?cid=learn-onpage-download-install-visual-studio-page-cta)
- [.NET](https://dotnet.microsoft.com/en-us/)
- [pgAdmin](https://www.pgadmin.org/)
- [PostgreSQL](https://www.postgresql.org/download/)

Follow these steps to set up the **Weather API**:
#### 1. Fork and Clone the Repository
Fork the repository, then clone it to your local machine.

```
git@github.com:yarelismartin/WeatherAPI.git
```
#### 2. Open in Visual Studio 2022
Open the solution file (`.sln`) in **Visual Studio 2022**.

#### 3. Restore Dependencies
Run the following command to restore project dependencies:

```
dotnet restore
```

#### 4. Configure User Secrets
Initialize user secrets and set your PostgreSQL connection string:
```
dotnet user-secrets init
dotnet user-secrets set "WeatherAPIDbConnectionString" "Host=localhost;Port=5432;Username=postgres;Password=<your_postgresql_password>;Database=WeatherAPI"
```
Replace `<your_postgresql_password>` with your actual database password.

The `appsettings.json` file is already pre-configured with:
- My OpenWeatherMap API key
- JWT secret for authentication
So no additional configuration is needed.

#### 5. Apply Migrations and Create the Database
Before running the update command, make sure the EF Core CLI is installed. This tool is required to apply the existing migrations:

```
dotnet tool install --global dotnet-ef
```

Once installed, apply the migrations to create the database:

```
dotnet ef database update
```

#### 6. Start Debugging
Run the project in debug mode by selecting the **Start Debugging** option in Visual Studio. This will launch the API, and you can access Swagger to test the endpoints.

#### 7. Test the API
You can use the built-in Swagger UI to test your endpoints.
This API uses JWT authentication.
**To test protected routes:**
- First, register and log in using the API.
- Copy the JWT token from the login response.
- Click "Authorize" (the lock icon in Swagger) and paste the token string.


### Models

#### User
| Property      | Type   | Description          |
|---------------|--------|----------------------|
| Id            | int    |                      |
| Username      | string |                      |
| Email         | string |                      |
| PasswordHash  | string | (Hashed)             |
| FavoriteLocations | List\<Location\> |        |

#### Location
| Property          | Type         | Description          |
|-------------------|--------------|----------------------|
| Id                | int          |                      |
| Name              | string       |                      |
| FavoritedByUsers  | List\<User\> |                      |


###  API Documentation

#### `POST /api/auth/register`
To register a user, you'll need a request body similar to the one below, containing a username, email, and password. For privacy protection, the password is not stored directly in the database. Instead, a hashedPassword property stores a version of the password that has gone through the process of being securely hashed using the BCrypt algorithm, ensuring that the original plain-text password cannot be easily retrieved or exposed.

**Request Body:**
```json
{
  "username": "username_example",
  "email": "example@example.com",
  "password": "yourPassword123"
}
```

##### Success Responses
- **201 Created**
  ```json
  {
    "message": "User registered successfully."
  }
  ```

##### Error Responses
- **409 Conflict**
  ```json
  {
    "message": "This email is already in use. Please try a different email.."
  }
  ```

- **500 Internal Server Error**
  ```json
  {
    "message": "An unexpected error occurred. Please try again later."
  }
  ```
---

#### `POST /api/auth/login`
This endpoint authenticates a user using their email and password. The password provided in the request body is hashed and then compared to the stored hash in the database. If the email exists and the hashed passwords match, a token is returned for use in authorized requests.

**Request Body:**
```json
{
  "email": "example@example.com",
  "password": "yourPassword123"
}
```
##### Success Responses
- **200 Ok**
  ```json
  {
    "token": "unr88ry238hiu892h3892h389hrun23r98jr98j3r92r092"
  }
  ```
Returned when the user is successfully authenticated. This token should be included in the headers of future requests to access protected endpoints.

##### Error Responses
- **409 Conflict**
  ```json
  {
    "message": "This email has not been registered in the database. Please try a different email"
  }
  ```
Returned when the provided email does not match any user in the database.


  - **409 Conflict**
  ```json
  {
    "message": "Invalid email or password."
  }
  ```
Returned when the email exists but the provided password does not match the stored hash.

 - **500 Internal Server Error**
  ```json
  {
    "message": "An unexpected error occurred. Please try again later."
  }
  ```

---
#### `GET /api/weather/{location}`
This endpoint retrieves the current weather data for the specified location using the OpenWeather API. When a request is made, the service first checks an in-memory cache to see if weather data already exists for the given location. If found, the cached data is returned. If not, the service makes a request to the OpenWeather API to fetch the current weather data and caches the result for future use.

##### Success Responses
- **200 OK**  
    ```json
    {
      "coord": {
        "lon": -86.7743,
        "lat": 36.1623
      },
      "weather": [
        {
          "id": 804,
          "main": "Clouds",
          "description": "overcast clouds",
          "icon": "04d"
        }
      ],
      "main": {
        "temp": 300.88,
        "feels_like": 303.54,
        "temp_min": 299.77,
        "temp_max": 301.48,
        "pressure": 1006,
        "humidity": 72,
        "sea_level": 1006,
        "grnd_level": 985
      },
      "wind": {
        "speed": 4.47,
        "deg": 230,
        "gust": 8.49
      },
      "clouds": {
        "all": 100
      },
      "sys": {
        "sunrise": 1747392005,
        "sunset": 1747442824
      },
      "name": "Nashville",
      "cod": 200
    }
    ```
    Weather data was successfully retrieved.


##### Error Responses
- **404 NotFound**
  ```json
  {
    "message": "City name cannot be empty."
  }
  ```
  The city string passed was empty.
  
- **404 NotFound**
    ```json
  {
    "message": "No location data found for this city."
  }
  ```
  No weather data was returned for the location the clinet passed.
  
 - **500 Internal Server Error**
  ```json
  {
    "message": "An unexpected error occurred. Please try again later."
  }
  ```
---
#### `GET /api/weather/forecast/{location}`
This endpoint retrieves the 5-day weather forecast for the specified location using the OpenWeather API. The service first checks an in-memory cache for existing forecast data for the location. If cached data is found, it is returned immediately. Otherwise, the service fetches fresh forecast data from the OpenWeather API, caches it for 30 minutes, and then returns it.

##### Success Responses
- **200 OK**  
    ```json
    {
      "cod": "200",
      "message": 0,
      "cnt": 40,
      "list": [
        {
          "dt": 1747440000,
          "main": {
            "temp": 300.88,
            "feels_like": 303.54,
            "temp_min": 300.41,
            "temp_max": 300.88,
            "pressure": 1006,
            "humidity": 72,
            "sea_level": 1006,
            "grnd_level": 985
          },
          "weather": [
            {
              "id": 500,
              "main": "Rain",
              "description": "light rain",
              "icon": "10d"
            }
          ],
          "clouds": {
            "all": 100
          },
          "wind": {
            "speed": 5.6,
            "deg": 212,
            "gust": 13.05
          },
          "visibility": 10000,
          "pop": 1,
          "rain": {
            "3h": 0.56
          },
          "sys": {
            "sunrise": 0,
            "sunset": 0
          },
          "dt_txt": "2025-05-17 00:00:00"
        }
        // more forecast items...
      ]
    }
    ```
##### Error Responses
- **404 NotFound**
  ```json
  {
    "message": "City name cannot be empty."
  }
  ```
  The city string passed was empty.

  - **404 NotFound**
  ```json
  {
    "message": "No location data found for this city."
  }
  ```
Geocoding failed or returned invalid coordinates (0,0).

  - **404 NotFound**
  ```json
  {
    "message": "Failed to parse forecast data for the city."
  }
  ```
 - **500 Internal Server Error**
  ```json
  {
    "message": "An unexpected error occurred. Please try again later."
  }
  ```

---
#### `POST /api/favorites`
This endpoint allows an authenticated user to add a location to their list of favorite locations. The client must send the ID of the location to be added in the request body. The user’s authentication token is also sent with the request to verify the user’s identity and ensure that only authorized users can modify their favorites list.

**Request Body:**
```json
{
  "locationId": 123
}
```
##### Success Responses
- **200 Ok**
  ```json
  {
    "message": "Location added to favorites."
  }
  ```

##### Error Responses
 - **401 Unauthorized**
  ```json
  {
    "message": "Unauthorized"
  }
  ```

 - **400 Bad Request**
  ```json
  {
    "message": "User not found."
  }
  ```

 - **400 Bad Request**
  ```json
  {
    "message": "Location is already in favorites."
  }
  ```
 - **500 Internal Server Errorr**
  ```json
  {
    "message": "An unexpected error occurred while adding the favorite."
  }
  ```

---
#### `GET /api/favorites`
This endpoint retrieves the list of favorite locations for the authenticated user. The client must include a valid authentication token with the request to verify the user's identity and authorize access to their favorite locations.

##### Success Responses
- **200 OK**  
    ```json
    [
      {
        "id": 1,
        "name": "New York City"
      },
      {
        "id": 3,
        "name": "Chicago"
      }
    ]
    ```

##### Error Responses
- **401 Unauthorized**
  ```json
  {
    "message": "Unauthorized."
  }
  ```
  
- **404 Not Found**
  ```json
  {
    "message": "No favorite locations found."
  }
  ```
- **500 Internal Server Errorr**
  ```json
  {
    "message": "An unexpected error occurred. Please try again later."
  }
  ```
  
---
#### `DELETE /api/favorites/{id}`
This endpoint allows an authenticated user to remove a location from their list of favorite locations by specifying the location's ID in the URL path. The client must include a valid authentication token to verify the user's identity and authorize the removal action.


##### Success Responses
- **204 No Content**  

##### Error Responses
- **Unauthorized**  
  ```json
  {
    "message": "Unauthorized."
  }
  ```

  - **400 Bad Request**
  ```json
  {
    "message": "User not found."
  }
  ```

    - **400 Bad Request**
  ```json
  {
    "message": "The location you are trying to remove is not in favorites."
  }
  ```
- **500 Internal Server Errorr**
  ```json
  {
    "message": "Failed to remove favorite."
  }
  ```
---
## Architecture Decisions

I created a RESTful API using the minimal API structure to maintain simplicity and scalability. The design follows a repository pattern that separates concerns by having the endpoint layer manage responses, status codes, and authorization checks with tokens. The service layer handles business logic and crafts messages tailored to different scenarios, while the repository layer takes care of data retrieval and modification. Logging is incorporated in both the service and endpoint layers to capture warnings, informational messages, and errors, which enhances traceability and debugging.

## Running Tests
In Visual Studio, navigate to the "Test" menu at the top, then select "Run All Tests". This will execute all unit tests and display the results in the Test Explorer.
