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
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
} from '@mui/icons-material';

interface Car {
  id: number;
  make: string;
  model: string;
  year: number;
  price: number;
  status: 'Available' | 'Sold' | 'Reserved';
}

const initialCars: Car[] = [
  {
    id: 1,
    make: 'Toyota',
    model: 'Camry',
    year: 2022,
    price: 25000,
    status: 'Available',
  },
  {
    id: 2,
    make: 'Honda',
    model: 'Civic',
    year: 2023,
    price: 22000,
    status: 'Sold',
  },
  {
    id: 3,
    make: 'Ford',
    model: 'Mustang',
    year: 2021,
    price: 35000,
    status: 'Reserved',
  },
];

export default function Inventory() {
  const [cars, setCars] = useState<Car[]>(initialCars);
  const [open, setOpen] = useState(false);
  const [selectedCar, setSelectedCar] = useState<Car | null>(null);
  const [formData, setFormData] = useState<Partial<Car>>({});

  const handleOpen = (car?: Car) => {
    if (car) {
      setSelectedCar(car);
      setFormData(car);
    } else {
      setSelectedCar(null);
      setFormData({});
    }
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
    setSelectedCar(null);
    setFormData({});
  };

  const handleSubmit = () => {
    if (selectedCar) {
      setCars(cars.map(car => 
        car.id === selectedCar.id ? { ...car, ...formData } : car
      ));
    } else {
      const newCar: Car = {
        id: cars.length + 1,
        make: formData.make || '',
        model: formData.model || '',
        year: formData.year || 2024,
        price: formData.price || 0,
        status: formData.status as Car['status'] || 'Available',
      };
      setCars([...cars, newCar]);
    }
    handleClose();
  };

  const handleDelete = (id: number) => {
    setCars(cars.filter(car => car.id !== id));
  };

  return (
    <Box sx={{ flexGrow: 1 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
        <Typography variant="h4">Inventory</Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={() => handleOpen()}
        >
          Add Car
        </Button>
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Make</TableCell>
              <TableCell>Model</TableCell>
              <TableCell>Year</TableCell>
              <TableCell>Price</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {cars.map((car) => (
              <TableRow key={car.id}>
                <TableCell>{car.make}</TableCell>
                <TableCell>{car.model}</TableCell>
                <TableCell>{car.year}</TableCell>
                <TableCell>${car.price.toLocaleString()}</TableCell>
                <TableCell>{car.status}</TableCell>
                <TableCell>
                  <IconButton onClick={() => handleOpen(car)}>
                    <EditIcon />
                  </IconButton>
                  <IconButton onClick={() => handleDelete(car.id)}>
                    <DeleteIcon />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      <Dialog open={open} onClose={handleClose}>
        <DialogTitle>
          {selectedCar ? 'Edit Car' : 'Add New Car'}
        </DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            label="Make"
            fullWidth
            value={formData.make || ''}
            onChange={(e) => setFormData({ ...formData, make: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Model"
            fullWidth
            value={formData.model || ''}
            onChange={(e) => setFormData({ ...formData, model: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Year"
            type="number"
            fullWidth
            value={formData.year || ''}
            onChange={(e) => setFormData({ ...formData, year: parseInt(e.target.value) })}
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
            label="Status"
            select
            fullWidth
            value={formData.status || 'Available'}
            onChange={(e) => setFormData({ ...formData, status: e.target.value as Car['status'] })}
            SelectProps={{
              native: true,
            }}
          >
            <option value="Available">Available</option>
            <option value="Sold">Sold</option>
            <option value="Reserved">Reserved</option>
          </TextField>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>Cancel</Button>
          <Button onClick={handleSubmit} variant="contained">
            {selectedCar ? 'Save' : 'Add'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
} 