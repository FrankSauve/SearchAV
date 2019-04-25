import * as React from 'react';
import auth from '../../Utils/auth';
import axios from 'axios';
import { HistoriqueListItem } from './HistoriqueListItem.tsx';


interface State {
    loading: boolean,
    unauthorized: boolean
}


export class TranscriptionHistorique extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            loading: false,
            unauthorized: false
        }
    }

    // Called when the component is rendered
    public componentDidMount() {

    }


    public render() {
        var i = 0;

        return (
            <div>
                <div className="box mg-top-30" id="historique-title-box">
                    <p className="historique-title"> HISTORIQUE </p>
                </div>
                <div className="box" id="historique-content-box">
                    <div>
                        {this.props.versions.map((version: any) => {
                            const listVersions = (
                                <HistoriqueListItem
                                    version={version}
                                    username={this.props.usernames[i]}
                                    versionToDiff={this.props.versionToDiff}
                                    updateDiff={this.props.updateDiff}
                                />
                            )
                            i++;
                            return listVersions;
                        })}
                    </div>
                </div>

            </div>
        );
    }
}