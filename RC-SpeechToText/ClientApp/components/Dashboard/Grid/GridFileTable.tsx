import * as React from 'react';
import GridFile from './GridFile';
import { Redirect } from 'react-router-dom';

interface State {
    files: any[],
    usernames: string[],
    loading: boolean,
    unauthorized: boolean
}

export default class GridFileTable extends React.Component<any, State> {

    constructor(props: any) {
        super(props);
        this.state = {
            files: this.props.files,
            usernames: this.props.usernames,
            loading: this.props.loading,
            unauthorized: false
        }
    }

    //Will update if props change
    public componentDidUpdate(prevProps : any) {
        if (this.props.files !== prevProps.files) {
            this.setState({ 'files': this.props.files });
        }

        if (this.props.usernames !== prevProps.usernames) {
            this.setState({ 'usernames': this.props.usernames });
        }
    }

    public updateFiles = () => {
        this.props.getAllFiles();
    }

    public render() {

        const progressBar = <img src="assets/loading.gif" alt="Loading..."/>

        var i = 0;

        return (
            <div>

                {this.state.unauthorized ? <Redirect to="/unauthorized" /> : null}
                
                {this.state.loading ? progressBar : null}
                <div className="columns is-multiline">
                    {this.state.files.map((file) => {
                        const FileComponent =
                          <GridFile 
                            file={file}
                            flag={file.flag}
                            title={file.title}
                            description={file.description}
                            username={this.state.usernames[i]}
                            filePath={file.filePath}
                            thumbnailPath={file.thumbnailPath}
                            dateAdded={file.dateAdded}
                            number={file.notified}
                            type={file.type}
                            key={file.id}
                            updateFiles={this.updateFiles}
                           />
                        i++; 
                        return(
                            FileComponent
                        )
                        
                    })}
                </div>
            </div>
        )
    }

    

}
