# Power Pages - Bring Your Own Code - React Sample

This is a react based sample application to demonstrate how a react website can be hosted on Power Pages and integrated with features like authentication, authorization, web apis etc.

This is a modern web application for managing car sales, inventory, and customer information.

## Features

- Authentication using Microsoft Entra Id (see [AuthButton.tsx](src/components/AuthButton.tsx) for implementation)
- Authorization using Power Pages web roles (see [Layout.tsx:45-49](src/components/Layout.tsx#L45-L49) for implementation)
- Fetching data from external data sources through [virtual tables](https://learn.microsoft.com/power-pages/configure/virtual-tables) and web apis (see [SalesLeads.tsx](src/pages/SalesLeads.tsx#L30-L63) for implementation)

## Prerequisites

- Node.js (v14 or higher)
- npm (v6 or higher)

## Installation

1. Clone the repository:

    ```powershell
    git clone https://github.com/microsoft/PowerApps-Samples
    cd portals/bring-your-own-code-samples\react-sample\
    ```

1. Install dependencies:

    ```powershell
    npm install
    ```

## Development

To start the development server:

```powershell
npm run dev
```

The application will be available at `http://localhost:5173`

## Building for Production

To create a production build:

```powershell
npm run build
```

The built files will be in the `dist` directory.

## Running on Power Pages

Below steps will help you run this app in Power Pages.

### Setup

1. Install [Microsoft Power Platform CLI](https://learn.microsoft.com/power-platform/developer/cli/introduction?tabs=windows#install-microsoft-power-platform-cli). (Version should be >= 1.43.6)
1. Allow `*.js` files by removing it from `Blocked Attachments` in `Privacy + Security` settings for your environment from Power Pages Admin Center.
1. Open a terminal and cd into `react-sample` folder.
1. Run `pac auth create --environment <Environment URL>` to login to your environment.
1. Run `pac solution import --path .\solutions\CodeSiteSample_1_0_0_1_managed.zip` to import the sample managed solution in your environment.
1. A **SalesLead** table will be created after the solution is imported. Please add some sample data to this table.

### Code changes

Update the value of `VITE_TENANT_ID` in [.env:1](.env#L1) with your tenant id.

### Uploading site to Power Pages

1. Open a terminal and cd into `react-sample` folder.
1. Run `npm run build` to build the code.
1. Run `pac pages upload-code-site --rootPath .` to upload the site to Power Pages.
1. Go to Power Pages home and click on **Inactive sites**.
1. You should see **Car Sales Management** site listed there. Click on **Reactivate** to proceed.
1. Once the site is activated, click on **Preview** to see it running on Power Pages.
1. Additionally, install [Power Platform Tools VS Code extension](https://aka.ms/power-platform-vscode) to easily upload the site in future iterations with a single click from within VS Code.

**Note:** Please login to the site to see **Sales Leads** page.

## Project Structure

```text
react-sample/
├── public/
│   └── car.svg
├── src/
│   ├── components/
│   │   ├── AuthButton.tsx
│   │   ├── Layout.tsx
│   │   └── ThemeToggle.tsx
|   ├── context/
│   │   ├── ThemeContext.tsx
│   ├── pages/
│   │   ├── Customers.tsx
│   │   ├── Dashboard.tsx
│   │   ├── Inventory.tsx
│   │   ├── Sales.tsx
│   │   └── SalesLeads.tsx
│   ├── App.tsx
│   ├── main.tsx
│   └── index.css
├── index.html
├── package.json
├── tsconfig.json
└── vite.config.ts
```

## Technologies Used

- [React](https://react.dev/)
- [TypeScript](https://www.typescriptlang.org/)
- [Material-UI](https://mui.com/material-ui/getting-started/installation/)
- [Vite](https://vite.dev/guide/)
- [React Router](https://reactrouter.com/)
- [Formik](https://formik.org/)
- [Yup](https://github.com/jquense/yup)

## License

MIT
