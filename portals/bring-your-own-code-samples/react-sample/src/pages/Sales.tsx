import { useState } from 'react';
import {
  Box,
  Button,
  Typography,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  MenuItem,
} from '@mui/material';
import { Add as AddIcon } from '@mui/icons-material';

interface Sale {
  id: number;
  carId: number;
  customerName: string;
  saleDate: string;
  price: number;
  paymentMethod: string;
}

const initialSales: Sale[] = [
  {
    id: 1,
    carId: 1,
    customerName: 'John Doe',
    saleDate: '2024-03-15',
    price: 25000,
    paymentMethod: 'Cash',
  },
  {
    id: 2,
    carId: 2,
    customerName: 'Jane Smith',
    saleDate: '2024-03-14',
    price: 22000,
    paymentMethod: 'Finance',
  },
];

const paymentMethods = ['Cash', 'Finance', 'Lease'];

export default function Sales() {
  const [sales, setSales] = useState<Sale[]>(initialSales);
  const [open, setOpen] = useState(false);
  const [formData, setFormData] = useState<Partial<Sale>>({});

  const handleOpen = () => {
    setFormData({});
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
    setFormData({});
  };

  const handleSubmit = () => {
    const newSale: Sale = {
      id: sales.length + 1,
      carId: formData.carId || 0,
      customerName: formData.customerName || '',
      saleDate: formData.saleDate || new Date().toISOString().split('T')[0],
      price: formData.price || 0,
      paymentMethod: formData.paymentMethod || 'Cash',
    };
    setSales([...sales, newSale]);
    handleClose();
  };

  return (
    <Box sx={{ flexGrow: 1 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
        <Typography variant="h4">Sales</Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={handleOpen}
        >
          Add Sale
        </Button>
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Car ID</TableCell>
              <TableCell>Customer Name</TableCell>
              <TableCell>Sale Date</TableCell>
              <TableCell>Price</TableCell>
              <TableCell>Payment Method</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {sales.map((sale) => (
              <TableRow key={sale.id}>
                <TableCell>{sale.carId}</TableCell>
                <TableCell>{sale.customerName}</TableCell>
                <TableCell>{sale.saleDate}</TableCell>
                <TableCell>${sale.price.toLocaleString()}</TableCell>
                <TableCell>{sale.paymentMethod}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      <Dialog open={open} onClose={handleClose}>
        <DialogTitle>Add New Sale</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            label="Car ID"
            type="number"
            fullWidth
            value={formData.carId || ''}
            onChange={(e) => setFormData({ ...formData, carId: parseInt(e.target.value) })}
          />
          <TextField
            margin="dense"
            label="Customer Name"
            fullWidth
            value={formData.customerName || ''}
            onChange={(e) => setFormData({ ...formData, customerName: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Sale Date"
            type="date"
            fullWidth
            value={formData.saleDate || ''}
            onChange={(e) => setFormData({ ...formData, saleDate: e.target.value })}
            InputLabelProps={{
              shrink: true,
            }}
          />
          <TextField
            margin="dense"
            label="Price"
            type="number"
            fullWidth
            value={formData.price || ''}
            onChange={(e) => setFormData({ ...formData, price: parseInt(e.target.value) })}
          />
          <TextField
            margin="dense"
            label="Payment Method"
            select
            fullWidth
            value={formData.paymentMethod || ''}
            onChange={(e) => setFormData({ ...formData, paymentMethod: e.target.value })}
          >
            {paymentMethods.map((method) => (
              <MenuItem key={method} value={method}>
                {method}
              </MenuItem>
            ))}
          </TextField>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>Cancel</Button>
          <Button onClick={handleSubmit} variant="contained">
            Add
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
} 