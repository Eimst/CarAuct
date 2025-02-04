# 🚀 CarAuct

A **car auction platform** with **real-time bidding** using **SignalR**. Users can **post cars** for auction by setting an **end time**, and other users can **place live bets**. The highest bid at the auction's end wins. The platform supports **filtering and sorting** of cars. The system is built using a **microservices architecture**, with each service having its **own database (MongoDB or PostgreSQL)** and communicating via **RabbitMQ**.

---

## 🔗 Live Demo & Screenshots

> ⚠️ **Note:**
>
> - This project is **deployed locally on Kubernetes** and does not have a live demo.

### 📸 Car Filtering & Sorting **(Video Demo)**
[![Watch Video](https://i.imgur.com/fzbOZ2w.png)](https://i.imgur.com/fzbOZ2w.mp4)


### 📹 Real-Time Bidding **(Video Demo)**
[![Watch Video](https://i.imgur.com/wdWkNA6.png)](https://i.imgur.com/wdWkNA6.mp4)


---

## 🛠 Tech Stack

- **Backend**: C#, ASP.NET Core Web API, MongoDB, PostgreSQL 
- **Frontend**: Next.js, TypeScript
- **Real-Time Communication**: SignalR for live bidding
- **Message Broker**: RabbitMQ for microservices communication
- **Ingress Controller**: for routing requests to the correct service
- **Hosting**: Locally deployed on **Kubernetes**

---

## ✨ Key Features

### ✅ **Implemented**

- 🚗 **Car Auction System**: Users can post cars and set an **auction end time**.
- 💬 **Real-Time Bidding with SignalR**: Bids update live without refreshing the page.
- 🔍 **Filtering & Sorting**: Users can **filter cars** by model, year, and price.
- 📦 **Microservices Architecture**: Each service operates independently with its own **MongoDB or PostgreSQL** database.
- 🔄 **Event-Driven Communication**: Services communicate via **RabbitMQ**, ensuring smooth interactions.
- 🌍 **Kubernetes Deployment**: The application runs locally on a **Kubernetes cluster**.
- 🌐 **Ingress Controller**: Directs user traffic to the correct microservice.
- 🔐 **Authentication & Authorization**: Identity Server using **OAuth2** and **OpenID Connect** via **Duende IdentityServer**.

---

### 🌍 **Hosting**

- **Frontend**: Served via Next.js and deployed in a Kubernetes pod.
- **Backend**: Microservices hosted within Kubernetes, communicating via RabbitMQ.

## 📚 Acknowledgments

This project was initially built following the **How to build a microservices based app using .Net, NextJS, IdentityServer, RabbitMQ running on Docker and Kubernetes** by **Neil Cummings** as part of my structured learning process.
