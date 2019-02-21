import * as React from 'react';


export class TranscriptionHistorique extends React.Component<any> {
    constructor(props: any) {
        super(props);
    }

    public render() {
        return (
            <div>
                <div className="Rectangle-Copy-5">
                      <p className="HISTORIQUE"> HISTORIQUE </p>
                </div>
                <div className="Rectangle-Copy-6">
                     <div className="content_historique">
                            <p className="title_historique_changes"> MODIFICATIONS </p>
                            <p className="historique_date"> 29-10-2018 11:45 </p>
                            <p className="historique_username"> John Doe </p>
                     </div>
                </div>
            </div>
        );
    }
}
