# Flowa Challenge

A microservices-based order management system using FIX 4.4 protocol for inter-service communication.

## Architecture

```
Browser → React Frontend (3000) → OrderGenerator.Api (8080) → FIX 4.4/TCP → OrderAccumulator (5001)
```

### Services

| Service | Technology | Port | Responsibility |
|---|---|---|---|
| `OrderGenerator.Frontend` | React + Vite + Nginx | 3000 | Order form and execution result display |
| `OrderGenerator.Api` | ASP.NET Core Web API | 8080 | Receives orders, sends FIX NewOrderSingle, returns ExecutionReport |
| `OrderAccumulator` | ASP.NET Core Worker Service | 5001 (FIX) | Accepts FIX messages, calculates financial exposure per symbol |

## FIX 4.4 Contract

Communication between `OrderGenerator.Api` and `OrderAccumulator` uses the FIX 4.4 protocol via QuickFIX/n.

| Message | Tag 35 | Direction | Description |
|---|---|---|---|
| NewOrderSingle | D | Generator → Accumulator | New order submission |
| ExecutionReport | 8 | Accumulator → Generator | Full fill confirmation |

### Key FIX Fields

| Field | Tag | Description |
|---|---|---|
| Symbol | 55 | Asset symbol (PETR4, VALE3, VIIA4) |
| Side | 54 | 1 = Buy, 2 = Sell |
| OrderQty | 38 | Order quantity |
| Price | 44 | Order price |
| ClOrdID | 11 | Unique order ID for correlation |
| OrdStatus | 39 | 2 = Filled |

## Financial Exposure

The `OrderAccumulator` calculates financial exposure per symbol:

```
Exposure = Σ(price × qty) buys - Σ(price × qty) sells
```

Buy orders increase exposure, sell orders decrease it.

## Running Locally

### Prerequisites

- Docker
- Docker Compose

### Start all services

```bash
docker compose up
```

Access the frontend at `http://localhost:3000`.

### Run without Docker

**OrderAccumulator:**
```bash
cd src/OrderAccumulator
dotnet run
```

**OrderGenerator.Api:**
```bash
cd src/OrderGenerator.Api
dotnet run
```

**Frontend:**
```bash
cd src/OrderGenerator.Frontend
npm install
npm run dev
```

Access the frontend at `http://localhost:5173`.

## Order Validation

| Field | Rule |
|---|---|
| Symbol | Must be PETR4, VALE3 or VIIA4 |
| Side | Must be Buy or Sell |
| Quantity | Positive integer, less than 100,000 |
| Price | Positive decimal, multiple of 0.01, less than 1,000 |

## Architectural Decisions

### FIX 4.4 for inter-service communication
FIX 4.4 was used as required by the challenge specification. In a real-world scenario, internal service-to-service communication would typically use Kafka or gRPC, with FIX reserved for external counterparty integration (exchanges, prime brokers).

### Request/Response Correlation
Since FIX is asynchronous, a `ConcurrentDictionary<ClOrdID, TaskCompletionSource>` bridges the synchronous HTTP request with the asynchronous `ExecutionReport` callback.

### decimal over double
All financial calculations use `decimal` to avoid binary floating-point precision errors inherent to `float` and `double`.

### Central Package Management
NuGet package versions are managed centrally via `Directory.Packages.props`, ensuring version consistency across all projects.

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | C# / ASP.NET Core 9 / Worker Service |
| FIX Protocol | QuickFIX/n 1.14.0 |
| Frontend | React 18 + Vite |
| Containerization | Docker + Docker Compose |
| CI/CD | GitLab CI (SAST + Secret Detection) |

---

This is a challenge by [Coodesh](https://coodesh.com)