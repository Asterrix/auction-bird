# AuctionBird

### Description
This is a full-stack application designed to simulate the "Auctioning" experience that you would find in a real-time production ready application.

**Disclaimer**: Because this is a "**sandbox**", it should be considered as such; it is not a production-ready program and was never intended to be, therefore numerous liberties were taken during the development process
but the idea behind the "Auctioning" experience stays the same.

### Technologies used

- **Frontend**: React + TypeScript, Tailwind
- **Backend**: .NET, PostgreSQL, Redis, Docker
- **Third Party API**: Stripe
- **Cloud Services**: AWS Cognito, Firebase Storage

### Features:

- User signin/signup
- Item Search
- Item filtration
- Creation of Items
- Full detailed display of Items
- Bidding functionality
- Notification system for bids
- Item recommendations

### Running the project:
**Disclaimer**: This project connects with a variety of third-party services, including AWS Cognito, Firebase Storage, Stripe, and others.\
To fully utilize the project's features, it's necessary to go through the dashboard process and set up each service individually.
This is because some services require a unique private key for communication.

Ignoring the essential keys for the project, it should be simple to set up on any device with Docker installed. The project includes a docker-compose.yml file, which should make executing the whole process simple.

Run the following commands in an empty folder:

1. Clone the repository:

   ```bash
   git clone https://github.com/enisnezirevic/auction-bird.git
   cd auction-bird

2. Build and start the containers:

    ```bash
    docker-compose up -d
   
3. Open the following link in the browser:

    ```bash
   http://localhost:3000/
   ```
\
\
To shut down the application, run the following command in the auction-bird directory
   ```bash
    docker-compose down
