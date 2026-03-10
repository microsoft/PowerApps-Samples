import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Layout from './components/Layout';
import Dashboard from './pages/Dashboard';
import Inventory from './pages/Inventory';
import Sales from './pages/Sales';
import Customers from './pages/Customers';
import SalesLeads from './pages/SalesLeads';
import { ThemeProvider } from './context/ThemeContext';

// Theme is now managed by ThemeContext

function App() {
  return (
    <Router>
      <ThemeProvider>
        <Layout>
          <Routes>
            <Route path="/" element={<Dashboard />} />
            <Route path="/inventory" element={<Inventory />} />
            <Route path="/sales" element={<Sales />} />
            <Route path="/customers" element={<Customers />} />
            <Route path="/leads" element={<SalesLeads />} />
          </Routes>
        </Layout>
      </ThemeProvider>
    </Router>
  );
}

export default App;