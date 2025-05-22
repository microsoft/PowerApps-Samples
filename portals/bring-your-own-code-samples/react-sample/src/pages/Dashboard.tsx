import { Grid, Typography, Box, Card, CardContent, LinearProgress } from '@mui/material';
import {
  DirectionsCar as CarIcon,
  AttachMoney as MoneyIcon,
  People as PeopleIcon,
  TrendingUp as TrendingUpIcon,
} from '@mui/icons-material';

const SummaryCard = ({ title, value, icon, color, progress }: {
  title: string;
  value: string | number;
  icon: React.ReactNode;
  color: string;
  progress?: number;
}) => (
  <Card
    sx={ {
      height: '100%',
      display: 'flex',
      flexDirection: 'column',
      position: 'relative',
      overflow: 'hidden',
      transition: 'transform 0.3s ease-in-out, box-shadow 0.3s ease-in-out',
      '&:hover': {
        transform: 'translateY(-5px)',
        boxShadow: '0 8px 16px rgba(0,0,0,0.1)',
        '& .summary-card-icon-container': {
          transform: 'scale(1.5) rotate(7deg)',
        },
        '& .summary-card-bg-pattern': {
          opacity: 0.15,
          transform: 'translate(20%, -20%) rotate(50deg) scale(1.2)',
        }
      },
    } }
  >
    <Box
      className="summary-card-bg-pattern"
      sx={ {
        position: 'absolute',
        top: 0,
        right: 0,
        width: '100px',
        height: '100px',
        opacity: 0.1,
        transform: 'translate(30%, -30%) rotate(45deg)',
        backgroundColor: color,
        transition: 'opacity 0.3s ease-in-out, transform 0.3s ease-in-out',
      } }
    />
    <CardContent sx={ { flexGrow: 1, position: 'relative', zIndex: 1 } }>
      <Box sx={ { display: 'flex', justifyContent: 'space-between', mb: 2 } }>
        <Typography color="text.secondary" variant="h6" sx={ { fontWeight: 500 } }>
          { title }
        </Typography>
        <Box
          className="summary-card-icon-container"
          sx={ {
            color,
            transform: 'scale(1.2)',
            transition: 'transform 0.3s ease-in-out'
          } }
        >
          { icon }
        </Box>
      </Box>
      <Typography component="p" variant="h4" sx={ { fontWeight: 600, mb: 1 } }>
        { value }
      </Typography>
      { progress !== undefined && (
        <Box sx={ { mt: 2 } }>
          <LinearProgress
            variant="determinate"
            value={ progress }
            sx={ {
              height: 8,
              borderRadius: 4,
              backgroundColor: `${color}20`,
              '& .MuiLinearProgress-bar': {
                backgroundColor: color,
              }
            } }
          />
          <Typography variant="body2" color="text.secondary" sx={ { mt: 1 } }>
            { progress }% of target
          </Typography>
        </Box>
      ) }
    </CardContent>
  </Card>
);

export default function Dashboard() {
  return (
    <Box sx={ { flexGrow: 1 } }>
      <Typography variant="h4" sx={ { mb: 4, fontWeight: 600 } }>
        Dashboard
      </Typography>
      <Grid container spacing={ 3 }>
        <Grid item xs={ 12 } sm={ 6 } md={ 3 }>
          <SummaryCard
            title="Total Inventory"
            value={ 42 }
            icon={ <CarIcon fontSize="large" /> }
            color="#1a237e"
            progress={ 75 }
          />
        </Grid>
        <Grid item xs={ 12 } sm={ 6 } md={ 3 }>
          <SummaryCard
            title="Monthly Sales"
            value="$124,500"
            icon={ <MoneyIcon fontSize="large" /> }
            color="#c2185b"
            progress={ 85 }
          />
        </Grid>
        <Grid item xs={ 12 } sm={ 6 } md={ 3 }>
          <SummaryCard
            title="Total Customers"
            value={ 156 }
            icon={ <PeopleIcon fontSize="large" /> }
            color="#2e7d32"
            progress={ 65 }
          />
        </Grid>
        <Grid item xs={ 12 } sm={ 6 } md={ 3 }>
          <SummaryCard
            title="Growth Rate"
            value="+12.5%"
            icon={ <TrendingUpIcon fontSize="large" /> }
            color="#ed6c02"
            progress={ 90 }
          />
        </Grid>
      </Grid>
    </Box>
  );
}
