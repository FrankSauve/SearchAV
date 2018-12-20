import * as React from 'react';
import { Route } from 'react-router-dom';
import { Home } from './components/Home';
import Navbar from './components/Navbar/Navbar';
import TestAccuracy from './components/Google/TestAccuracy';
import Transcription from './components/Google/Transcription';
import TranscriptionOld from './components/Google/TranscriptionOld';
import VideoTable from './components/MyVideos/VideoTable'

//TODO: Remove TranscriptionOld
export const routes = <div>
    <Route path='/' component = { Navbar } />
    <Route exact path='/' component={Home} />
    <Route exact path="/TestAccuracy" component={TestAccuracy} />
    <Route exact path="/Transcription" component={Transcription} />
    <Route exact path="/TranscriptionOld" component={TranscriptionOld} /> 
    <Route exact path="/videos" component={ VideoTable } />
</div>;
