# Power Pages - Bring Your Own Code - React Sample

This is a react based sample application to demonstrate how a react website can be hosted on Power Pages and integrate with features like authentication, authorization, web apis etc.

This is a modern web application for managing car sales, inventory, and customer information.

## Features

- Authentication using Microsoft Entra Id (see [AuthButton.tsx](src/components/AuthButton.tsx) for implementation)
- Authorization using Power Pages web roles (see [Layout.tsx:45-49](src/components/Layout.tsx#L45-L49) for implementation)
- Fetching data from external data sources through [virtual tables](https://learn.microsoft.com/en-us/power-pages/configure/virtual-tables) and web apis (see [SalesLeads.tsx](src/pages/SalesLeads.tsx#L30-L63) for implementation)

## Prerequisites

- Node.js (v14 or higher)
- npm (v6 or higher)

## Installation

1. Clone the repository:

    ```bash
    git clone https://github.com/microsoft/PowerApps-Samples
    cd portals/bring-your-own-code-samples\react-sample\
    ```

1. Install dependencies:

    ```bash
    npm install
    ```

## Development

To start the development server:

```bash
npm run dev
```

The application will be available at `http://localhost:5173`

## Building for Production

To create a production build:

```bash
npm run build
```

The built files will be in the `dist` directory.

## Running on Power Pages

Below steps will help you run this app in Power Pages.

### Setup

1. Install [Microsoft Power Platform CLI](https://learn.microsoft.com/en-us/power-platform/developer/cli/introduction?tabs=windows#install-microsoft-power-platform-cli). (Version should be >= 1.43.6)
1. Allow `*.js` files by removing it from `Blocked Attachments` in `Privacy + Security` settings for your environment from Power Pages Admin Center.
1. Create a new web role called `Sales Person`. See [Create and assign web roles](https://learn.microsoft.com/en-us/power-pages/security/create-web-roles).
1. Create a new table called `SalesLead` with below columns, add some dummy data and create table permissions. See [Configuring table permissions](https://learn.microsoft.com/en-us/power-pages/security/table-permissions).
    1. Account Id
    1. Name
    1. Type
    1. Description
    1. Amount
1. Enable web api for the above table. See [How to: Use portal Web API](https://learn.microsoft.com/en-us/power-pages/configure/webapi-how-to).

### Code changes

1. Update your tenant id in [AuthButton.tsx:41](src/components/AuthButton.tsx#L41).
1. Update web api entity set name in [SalesLeads.tsx:32](src/pages/SalesLeads.tsx#L32).
1. Update mapping according to your columns in [SalesLeads.tsx:50-54](src/pages/SalesLeads.tsx#L50-L54).

### Uploading site to Power Pages

1. Open a terminal and cd into `react-sample` folder.
1. Run `npm run build` to build the code.
1. Run `pac pages upload-code-site --rootPath .` to upload the site to Power Pages.
1. Go to Power Pages home and click on `Inactive Sites`.
1. You should see `Car Sales Management` site listed there. Click on `Reactivate` to proceed.
1. Once the site is activated, click on `Preview` to see it running on Power Pages.
1. Additionally, install [Power Platform Tools VS Code extension](https://aka.ms/power-platform-vscode) to easily upload the site in future iterations with a single click.


## Project Structure

```
react-sample/
├── public/
│   └── car.svg
├── src/
│   ├── components/
│   │   └── Layout.tsx
│   │   ├── pages/
│   │   │   ├── Customers.tsx
│   │   │   ├── Dashboard.tsx
│   │   │   ├── Inventory.tsx
│   │   │   ├── Sales.tsx
│   │   │   └── SalesLeads.tsx
│   │   ├── App.tsx
│   │   ├── main.tsx
│   │   └── index.css
│   ├── index.html
│   ├── package.json
│   ├── tsconfig.json
│   └── vite.config.ts
```

## Technologies Used

- React
- TypeScript
- Material-UI
- Vite
- React Router
- Formik
- Yup

## License

MIT
