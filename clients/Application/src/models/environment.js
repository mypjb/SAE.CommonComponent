import { useCallback, useEffect, useState } from "react";
import { dict } from "../utils/service";

export default () => {
    const [state, setState] = useState([]);
    const load = useCallback(() => {
        dict.env()
            .then((data) => {
                setState(data);
            });
    }, []);

    useEffect(() => {
        load();
    }, []);

    return {
        state,
        load
    }
}