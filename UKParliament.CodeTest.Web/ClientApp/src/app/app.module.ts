import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
//import { HomeComponent } from './components/home/home.component';
//import { SignInComponent } from './components/sign-in/sign-in.component';
import { PersonComponent } from './components/person/person.component';

@NgModule({ declarations: [
        AppComponent,
        //HomeComponent,
        //SignInComponent,
        PersonComponent
    ],
    bootstrap: [AppComponent], imports: [BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        FormsModule,
        RouterModule.forRoot([
          //{ path: '', component: HomeComponent, pathMatch: 'full' }
          //{ path: '', component: SignInComponent, pathMatch: 'full' }
          { path: '', component: PersonComponent, pathMatch: 'full' }
        ])], providers: [provideHttpClient(withInterceptorsFromDi())] })
export class AppModule { }
