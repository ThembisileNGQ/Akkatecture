# Akkatecture Walkthrough Sample

This is the sample that shows the completed result of the [walkthrough](https://akkatecture.net/docs/walkthrough-introduction) exercise..

## Akkatecture.Walkthrough.Domain

This is the domain of the sample which follows from the walkthrough tutorial.

#### Commands
* **OpenNewAccountCommand** - Command for creating a new bank account.
* **ReceiveMoneyCommand** - Command for adding funds to a bank account.
* **TransferMoneyCommand** - Command for sending funds from a bank account.

#### Events
* **AccountOpenedEvent** - Event that denotes an account has been opened.
* **FeesDeductedEvent** - Event that denotes that bank fees have been deducted from an account.
* **MoneyReceivedEvent** - Event that denotes that a mank account has received funds.
* **MoneySentEvent** - Event that denotes that a bank account has sent funds.
* **MoneyTransferCompletedEvent** - Event that denotes that a transfer has ended.
* **MoneyTransferStartedEvent** - Event that denotes that a money transfer has started.

## Akkatecture.Walkthrough.Application

This application is incharge of instantiating and running the walkthrough example.

### Description

The application instantiates all the necessary actors and domain entities required. It then creates two  bank accounts using the 
`OpenNewAccountCommand`s for the sender and receiver. Finally, the sender makes an intent to send money to the receiver through a 
`TransferMoneyCommand`. On sucessful execution of this command, the sender account emits a `MoneySentEvent` that will start a `MoneyTransferSaga`.
The `MoneyTransferSaga` coordinates and fascilitates with the money transfer and tells the receicer account to accept the money. Meanwhile,
The `RevenueSubscriber`, which is subscribed to the `FeesDeductedEvent`, will listen to all of these events and aggregate a "revenue" that the bank has earned.
This result is aggregated into a mock repository called the `RevenueRepository`. We then query the `RevenueRepository` using a `GetRevenueQuery`.

> to run the application in jetbrains rider or visual studio code, run the `Akkatecture.Walkthrough.Application` configuration in the IDE.