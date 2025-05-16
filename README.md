# WeatherAPI

## Table of Contents
- [Project Overview](#project-overview)
- [Setup Instructions](#setup-instructions)
- [API Documentation](#api-documentation)
  - [Models](#models)
    - [User](#user)
    - [Location](#location)
    - [WeatherData](#weatherdata)
    - [ForecastResponse](#forecastresponse)
  - [Endpoints](#endpoints)
    - [POST /api/auth/register](#post-apiauthregister)
    - [POST /api/auth/login](#post-apiauthlogin)
    - [GET /api/weather/current?city={city}](#get-apiweathercurrentcitycity)
    - [GET /api/weather/forecast?city={city}](#get-apiweatherforecastcitycity)
    - [POST /api/favorites](#post-apifavorites)
    - [GET /api/favorites](#get-apifavorites)
    - [DELETE /api/favorites/{id}](#delete-apifavoritesid)
- [Architecture Decisions](#architecture-decisions)
- [Running Tests](#running-tests)

---

## Setup Instructions

## API Documentation

### Models

#### User
| Property | Type   | Description          |
|----------|--------|----------------------|
| Id       | int    |                      |
| Email    | string |                      |
| Password | string | (Hashed)             |
| FavoriteLocations | List\<Location\> |   |

#### Location
| Property | Type   | Description          |
|----------|--------|----------------------|
| Id       | int    |                      |
| Name     | string |                      |

#### WeatherData
| Property | Type   | Description          |
|----------|--------|----------------------|
| Temp     | float  |                      |
| Summary  | string |                      |
| Wind     | float  |                      |
| ...      | ...    |                      |

#### ForecastResponse
| Property | Type   | Description          |
|----------|--------|----------------------|
| List     | List\<WeatherData\> |        |
| City     | string |                      |

### Endpoints

#### POST /api/auth/register
##### Success Responses

##### Error Responses

#### POST /api/auth/login
##### Success Responses

##### Error Responses

#### GET /api/weather/current?city={city}
##### Success Responses

##### Error Responses

#### GET /api/weather/forecast?city={city}
##### Success Responses

##### Error Responses

#### POST /api/favorites
##### Success Responses

##### Error Responses

#### GET /api/favorites
##### Success Responses

##### Error Responses

#### DELETE /api/favorites/{id}
##### Success Responses

##### Error Responses

## Architecture Decisions

## Running Tests
