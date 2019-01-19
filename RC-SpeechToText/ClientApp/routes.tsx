import * as React from 'react';
import { Route } from 'react-router-dom';
import { Home } from './components/Home';
import Unauthorized from './components/Unauthorized';
import Navbar from './components/Navbar/Navbar';
import TestAccuracy from './components/Google/TestAccuracy';
import Transcription from './components/Google/Transcription'; 
import FileTable from './components/MyFiles/FileTable'


export const routes = <div>
    <Route path='/' component={ Navbar } />
    <Route exact path="/unauthorized" component={Unauthorized} />
    <Route exact path='/' component={Home} />
    <Route exact path="/TestAccuracy" component={TestAccuracy} />
    <Route exact path="/Transcription" component={Transcription} />
    <Route exact path="/videos" component={ FileTable } />
</div>;
