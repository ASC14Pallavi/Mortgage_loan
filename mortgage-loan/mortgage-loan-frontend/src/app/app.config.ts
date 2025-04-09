import { ApplicationConfig, importProvidersFrom, provideZoneChangeDetection } from '@angular/core';
import { provideRouter, RouterModule } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations'; 
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { AuthInterceptor } from './core/interceptors/auth.interceptors';
import {FormsModule} from '@angular/forms';
import { provideToastr } from 'ngx-toastr';


export const appConfig: ApplicationConfig = {
  providers: [provideZoneChangeDetection({ eventCoalescing: true }), provideRouter(routes),importProvidersFrom(RouterModule.forRoot(routes)), provideHttpClient(),
    provideAnimations(), provideHttpClient(withInterceptors([AuthInterceptor])),importProvidersFrom(FormsModule),
    provideAnimations(), 
    provideToastr({ 
      positionClass: 'toast-top-right',
      timeOut: 3000,
      preventDuplicates: true ,
      progressBar: true,
      progressAnimation: 'increasing',
      closeButton: true, 
      easing: 'ease-in-out', 
      easeTime: 400
    })]
    };


