import React, { useState } from 'react';
import {
  Box,
  Typography,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  CircularProgress,
  Alert,
} from '@mui/material';

interface Lead {
  accountId: number;
  name: string;
  type: string;
  description: string;
  amount: string;
}

const SalesLeads: React.FC = () => {
  const [leads, setLeads] = useState<Lead[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isUnauthorized, setIsUnauthorized] = useState(false);

  const fetchLeads = async () => {
    try {
      const response = await fetch("/_api/sample_salesleads?$select=sample_accountid,sample_name,sample_type,sample_description,sample_amount");

      if (response.status === 403) {
        setIsUnauthorized(true);
        return [];
      }

      if (!response.ok) {
        throw new Error(`API call failed with status: ${response.status}`);
      }

      const data = await response.json();
      const leads = data.value;
      const returnData: Lead[] = [];

      for (let i = 0; i < leads.length; i++) {
        const lead = leads[i];
        const accountId = lead.sample_accountid;
        const name = lead.sample_name;
        const type = lead.sample_type;
        const description = lead.sample_description;
        const amount = lead.sample_amount;
        returnData.push({ accountId, name, type, description, amount });
      }

      return returnData;
    } catch (error) {
      console.error('Error fetching leads:', error);
      throw error;
    }
  };

  React.useEffect(() => {
    const loadLeads = async () => {
      try {
        setIsLoading(true);
        setIsUnauthorized(false);
        const leads = await fetchLeads();
        setLeads(leads);
      } catch (error) {
        console.error('Error fetching leads:', error);
        // You might want to show an error state here
      } finally {
        setIsLoading(false);
      }
    };
    loadLeads();
  }, []);

  return (
    <Box sx={{ p: 3 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
        <Typography variant="h4" gutterBottom>
          Sales Leads
        </Typography>
      </Box>

      {isUnauthorized && (
        <Alert severity="error" sx={{ mb: 2 }}>
          You are not authorized to view this data. Please contact your administrator.
        </Alert>
      )}

      {isLoading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}>
          <CircularProgress />
        </Box>
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Account Id</TableCell>
                <TableCell>Name</TableCell>
                <TableCell>Type</TableCell>
                <TableCell>Description</TableCell>
                <TableCell>Amount</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {leads.map((lead) => (
                <TableRow key={lead.accountId}>
                  <TableCell>{lead.accountId}</TableCell>
                  <TableCell>{lead.name}</TableCell>
                  <TableCell>{lead.type}</TableCell>
                  <TableCell>{lead.description}</TableCell>
                  <TableCell>{lead.amount}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Box>
  );
};

export default SalesLeads;
