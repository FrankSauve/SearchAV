import * as React from 'react';
import axios from 'axios';
import Video from './Video';
import { RouteComponentProps } from 'react-router';

interface State {
    videos: any[]
}

export default class VideoTable extends React.Component<RouteComponentProps<{}>, State> {

    constructor(props: any) {
        super(props);
        this.state = {
            videos: []
        }
    }

    public componentDidMount() {
        this.getAllVideos()
    }

    public getAllVideos = () => {
        const config = {
            headers: {
                'content-type': 'multipart/form-data'
            }
        }
        axios.get('/api/video/index', config)
            .then(res => {
                this.setState({'videos': res.data});
            });
    }

    public render() {
        return (
            <div className="container">
                <div className="columns is-multiline">
                    {this.state.videos.map((video) => {
                        return (
                            <Video 
                                videoId = {video.videoId}
                                title = {video.title}
                                videoPath = {video.videoPath}
                                transcription = {video.transcription}
                                dateAdded = {video.dateAdded}
                                key = {video.videoId}
                            />
                        )
                    })}
                </div>
            </div>
        )
    }

    

}
