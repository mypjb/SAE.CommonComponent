import { Modal } from "antd"

//build options
const buildOptions = (props, model) => {
    const handler = {
        onOk: (e) => { console.warn("Use default onOk"); },
        onCancel: (e) => { console.warn("Use default onCancel"); },
    };
    const okCallback = function (fn) {
        if (fn.onOk && fn.onCancel) {
            handler.onOk = fn;
            handler.onCancel = fn.onCancel;
        } else {
            handler.onOk = fn;
        }
    };

    const options = {
        ...props,
        onOk: (e) => {
            if (props.onOk) {
                props.onOk(e);
            }
            if (handler.onOk(e) !== false) {
                e();
            }
        },
        onCancel: (e) => {
            let result = true;
            if (props.onCancel) {
                props.onCancel(e);
            }
            result = handler.onCancel(e);
            if (result !== false) {
                e();
            }
        }
    };

    if (props.contentElement) {
        const contentProps = {
            ...props.contentProps,
            okCallback,
            closeCallback: () => {
                model.destroy();
            }
        };
        options.content = (<props.contentElement {...contentProps}></props.contentElement>);
    }

    return options;
};

const FormModal = function (props) {
    const [modal, contextHolder] = Modal.useModal();
    const options = buildOptions(props, modal);
    modal.confirm(options);
    return contextHolder;
}

FormModal.confirm = function (props) {
    const model = Modal.confirm();
    const options = buildOptions(props, model);
    model.update(options);
};

export default FormModal;