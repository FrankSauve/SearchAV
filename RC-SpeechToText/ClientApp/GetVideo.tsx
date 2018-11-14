import './css/site.css';
import 'bootstrap';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { AppContainer } from 'react-hot-loader';
import { BrowserRouter, Link } from 'react-router-dom';
import * as RoutesModule from './routes';
import { RouteComponentProps } from 'react-router';
let routes = RoutesModule.routes;

interface GetVideoDataState {
empList: VideoData[];
loading: boolean;
} 

export class GetVideo extends React.Component<RouteComponentProps<{}>, GetVideoDataState>
{
    constructor(props: any) {
    super(props);
    this.state = { empList: [], loading: true };

    fetch('api/VideoController/GetAllVideos')
        .then(response => response.json() as Promise<VideoData[]>)
        .then(data => {
            this.setState({ empList: data, loading: false });
        });

    this.handleDelete = this.handleDelete.bind(this);

}

public render() {
    let contents = this.state.loading
        ? <p><em>Loading...</em></p>
        : this.renderVideoTable(this.state.empList);

    return <div>
        <h1>Video Data</h1>
        <p>All Videos</p>
        <p>
            <Link to="/addVideo">Create New</Link>
        </p>
        {contents}
    </div>;
} 

private handleDelete(id: number) {
    if (!confirm("Do you want to delete Video with Id: " + id))
        return;
    else {
        fetch('api/Video/Delete/' + id, {
            method: 'delete'
        }).then(data => {
            this.setState(
                {
                    empList: this.state.empList.filter((rec) => {
                        return (rec.videoId != id);
                    })
                });
        });
    }
} 

private renderVideoTable(empList: VideoData[]) {
    return <table className='table'>
        <thead>
            <tr>
                <th></th>
                <th>VideoId</th>
                <th>Name</th>
                <th>Path</th>
            </tr>
        </thead>
        <tbody>
            {empList.map(vid =>
                <tr key={vid.videoId}>
                    <td></td>
                    <td>{vid.videoId}</td>
                    <td>{vid.name}</td>
                    <td>{vid.path}</td>
                    <td>
                        <a className="action" onClick={(id) => this.handleDelete(vid.videoId)}>Delete</a>
                    </td>
                </tr>
            )}
        </tbody>
    </table>;
}
} 

export class VideoData {
videoId: number = 0;
name: string = "";
path: string = "";
}


