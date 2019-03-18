import * as React from 'react'

export class LoadingModal extends React.Component<any> {
    constructor(props: any) {
        super(props);
    }

    render() {
        return (
            <div className={`modal ${this.props.showModal ? "is-active" : null}`} >
                <div className="modal-background"></div>
                <img width="100" height="100" src="assets/loading.gif" alt="Loading..." />
            </div>
        )
    }
}
