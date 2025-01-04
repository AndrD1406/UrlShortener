import { Routes } from '@angular/router';
import { AppComponent } from './app.component';
import { UrlComponent} from './components/url/url.component';
import { RegisterComponent } from './components/register/register.component';
import { LoginComponent } from './components/login/login.component';

export const routes: Routes = [
  { path: "", redirectTo: "/login", pathMatch: "full" },
  { path: "url", component: UrlComponent },
  { path: "register", component: RegisterComponent },
  { path: "login", component: LoginComponent },
  { path: "logout", component: AppComponent }
];
