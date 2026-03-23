import { Route } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';


export const routes: Route[] = [
  { path: 'login', component: LoginComponent },
  { path: 'dashboard', component: DashboardComponent},
  { path: '', redirectTo: '/login', pathMatch: 'full' }
];